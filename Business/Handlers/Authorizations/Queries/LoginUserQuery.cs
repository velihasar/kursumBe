using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Authorizations.Queries
{
    public class LoginUserQuery : IRequest<IDataResult<AccessToken>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, IDataResult<AccessToken>>
        {
            private readonly IUserRepository _userRepository;
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly ITenantRepository _tenantRepository;
            private readonly IPersonRepository _personRepository;
            private readonly ITokenHelper _tokenHelper;
            private readonly IMediator _mediator;
            private readonly ICacheManager _cacheManager;

            public LoginUserQueryHandler(
                IUserRepository userRepository,
                ITenantUserRepository tenantUserRepository,
                ITenantRepository tenantRepository,
                IPersonRepository personRepository,
                ITokenHelper tokenHelper,
                IMediator mediator,
                ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _tenantUserRepository = tenantUserRepository;
                _tenantRepository = tenantRepository;
                _personRepository = personRepository;
                _tokenHelper = tokenHelper;
                _mediator = mediator;
                _cacheManager = cacheManager;
            }

            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<AccessToken>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
            {
                var input = request.Email?.Trim();
                var user = await _userRepository.GetAsync(u => (u.Email == input || (!string.IsNullOrEmpty(u.MobilePhones) && u.MobilePhones == input)) && u.Status);

                if (user == null)
                {
                    return new ErrorDataResult<AccessToken>(Messages.UserNotFound);
                }

                if (!HashingHelper.VerifyPasswordHash(request.Password, user.PasswordSalt, user.PasswordHash))
                {
                    return new ErrorDataResult<AccessToken>(Messages.PasswordError);
                }

                var claims = _userRepository.GetClaims(user.UserId);

                // Person / Role detection (Parent / Teacher)
                var person = _personRepository.Query()
                    .Include(p => p.Teacher)
                    .Include(p => p.Parent).ThenInclude(pr => pr.Students)
                    .FirstOrDefault(p => p.UserId == user.UserId && p.IsDeleted == false);

                if (person != null)
                {
                    if (person.Parent != null)
                    {
                        if (!claims.Any(c => c.Name == "Parent" || c.Name == "Veli"))
                        {
                            claims.Add(new Core.Entities.Concrete.OperationClaim { Name = "Parent" });
                        }
                        claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"ParentId:{person.Parent.Id}" });
                        var firstStudent = person.Parent.Students?.FirstOrDefault(s => s.IsDeleted == false)?.StudentId;
                        if (firstStudent.HasValue && firstStudent.Value > 0)
                        {
                            claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"StudentId:{firstStudent.Value}" });
                        }
                    }
                    if (person.Teacher != null)
                    {
                        if (!claims.Any(c => c.Name == "Teacher" || c.Name == "Ogretmen"))
                        {
                            claims.Add(new Core.Entities.Concrete.OperationClaim { Name = "Teacher" });
                        }
                        claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"TeacherId:{person.Teacher.Id}" });
                    }
                }

                var tenantUser = await _tenantUserRepository.GetAsync(tu => tu.UserId == user.UserId && tu.IsDeleted == false);
                if (tenantUser != null)
                {
                    claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"TenantId:{tenantUser.TenantId}" });
                }
                else
                {
                    var isSuperAdmin = claims.Any(c => c.Name != null && (c.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || c.Name.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase)));
                    if (!isSuperAdmin)
                    {
                        var tenant = await _tenantRepository.GetAsync(t => t.CreatedBy == user.UserId && t.IsDeleted == false);
                        if (tenant != null)
                        {
                            claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"TenantId:{tenant.Id}" });
                        }
                        else
                        {
                            var firstTenant = _tenantRepository.Query().FirstOrDefault(t => t.IsDeleted == false && t.IsActive == true);
                            if (firstTenant != null)
                            {
                                claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"TenantId:{firstTenant.Id}" });
                            }
                        }
                    }
                }

                var accessToken = _tokenHelper.CreateToken<DArchToken>(user, claims);
                accessToken.Claims = claims.Select(x => x.Name).ToList();

                user.RefreshToken = accessToken.RefreshToken;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                _cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", claims.Select(x => x.Name));

                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
    }
}