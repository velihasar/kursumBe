using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Authorizations.Queries
{
    public class LoginWithRefreshTokenQuery : IRequest<IResult>
    {
        public string RefreshToken { get; set; }

        public class LoginWithRefreshTokenQueryHandler : IRequestHandler<LoginWithRefreshTokenQuery, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly ITenantRepository _tenantRepository;
            private readonly ITokenHelper _tokenHelper;
            private readonly ICacheManager _cacheManager;

            public LoginWithRefreshTokenQueryHandler(
                IUserRepository userRepository,
                ITenantUserRepository tenantUserRepository,
                ITenantRepository tenantRepository,
                ITokenHelper tokenHelper,
                ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _tenantUserRepository = tenantUserRepository;
                _tenantRepository = tenantRepository;
                _tokenHelper = tokenHelper;
                _cacheManager = cacheManager;
            }

            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(LoginWithRefreshTokenQuery request, CancellationToken cancellationToken)
            {
                var userToCheck = await _userRepository.GetByRefreshToken(request.RefreshToken);
                if (userToCheck == null)
                {
                    return new ErrorDataResult<User>(Messages.UserNotFound);
                }

                var claims = _userRepository.GetClaims(userToCheck.UserId);
                var tenantUser = await _tenantUserRepository.GetAsync(tu => tu.UserId == userToCheck.UserId && tu.IsDeleted == false);
                if (tenantUser != null)
                {
                    claims.Add(new Core.Entities.Concrete.OperationClaim { Name = $"TenantId:{tenantUser.TenantId}" });
                }
                else
                {
                    var isSuperAdmin = claims.Any(c => c.Name != null && (c.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || c.Name.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase)));
                    if (!isSuperAdmin)
                    {
                        var tenant = await _tenantRepository.GetAsync(t => t.CreatedBy == userToCheck.UserId && t.IsDeleted == false);
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

                _cacheManager.Remove($"{CacheKeys.UserIdForClaim}={userToCheck.UserId}");
                _cacheManager.Add($"{CacheKeys.UserIdForClaim}={userToCheck.UserId}", claims.Select(x => x.Name));
                var accessToken = _tokenHelper.CreateToken<AccessToken>(userToCheck, claims);
                userToCheck.RefreshToken = accessToken.RefreshToken;
                _userRepository.Update(userToCheck);
                await _userRepository.SaveChangesAsync();
                return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            }
        }
	}
}

