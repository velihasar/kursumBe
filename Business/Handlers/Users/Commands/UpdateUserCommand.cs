using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Users.Commands
{
    public class UpdateUserCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string MobilePhones { get; set; }
        public int? TenantId { get; set; }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly ITenantUserRepository _tenantUserRepository;

            public UpdateUserCommandHandler(IUserRepository userRepository, ITenantUserRepository tenantUserRepository)
            {
                _userRepository = userRepository;
                _tenantUserRepository = tenantUserRepository;
            }


            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("GetUsers")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var isThereAnyUser = await _userRepository.GetAsync(u => u.UserId == request.UserId);

                var cleanMobile = !string.IsNullOrWhiteSpace(request.MobilePhones) ? request.MobilePhones.Replace(" ", "").Trim() : null;

                isThereAnyUser.FullName = request.FullName;
                isThereAnyUser.Email = request.Email;
                isThereAnyUser.MobilePhones = cleanMobile;

                _userRepository.Update(isThereAnyUser);
                await _userRepository.SaveChangesAsync();

                if (request.TenantId.HasValue)
                {
                    var existingTenantUser = _tenantUserRepository.Query().FirstOrDefault(tu => tu.UserId == request.UserId && tu.IsDeleted == false);
                    if (request.TenantId.Value > 0)
                    {
                        if (existingTenantUser != null)
                        {
                            existingTenantUser.TenantId = request.TenantId.Value;
                            existingTenantUser.IsActive = true;
                            existingTenantUser.UpdatedDate = System.DateTime.Now;
                            _tenantUserRepository.Update(existingTenantUser);
                        }
                        else
                        {
                            _tenantUserRepository.Add(new Core.Entities.Concrete.Project.TenantUser
                            {
                                UserId = request.UserId,
                                TenantId = request.TenantId.Value,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedDate = System.DateTime.Now
                            });
                        }
                        await _tenantUserRepository.SaveChangesAsync();
                    }
                    else if (existingTenantUser != null)
                    {
                        existingTenantUser.IsDeleted = true;
                        existingTenantUser.UpdatedDate = System.DateTime.Now;
                        _tenantUserRepository.Update(existingTenantUser);
                        await _tenantUserRepository.SaveChangesAsync();
                    }
                }

                return new SuccessResult(Messages.Updated);
            }
        }
    }
}