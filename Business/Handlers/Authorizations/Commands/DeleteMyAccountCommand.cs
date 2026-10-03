using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System;

namespace Business.Handlers.Authorizations.Commands
{
    public class DeleteMyAccountCommand : IRequest<IResult>
    {
        public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPersonRepository _personRepository;
            private readonly IUserGroupRepository _userGroupRepository;
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;

            public DeleteMyAccountCommandHandler(
                IUserRepository userRepository,
                IPersonRepository personRepository,
                IUserGroupRepository userGroupRepository,
                ITenantUserRepository tenantUserRepository,
                IMediator mediator)
            {
                _userRepository = userRepository;
                _personRepository = personRepository;
                _userGroupRepository = userGroupRepository;
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = UserInfoExtensions.GetUserIdOrZero();
                if (currentUserId <= 0)
                {
                    return new ErrorResult("Oturum bilgisi doğrulanamadı.");
                }

                var user = await _userRepository.GetAsync(u => u.UserId == currentUserId);
                if (user == null)
                {
                    return new ErrorResult("Kullanıcı bulunamadı.");
                }

                // 1. Unlink Person record from User (preserves institutional Person & Parent data)
                var person = await _personRepository.GetAsync(p => p.UserId == currentUserId && p.IsDeleted == false);
                if (person != null)
                {
                    person.UserId = null;
                    _personRepository.Update(person);
                    await _personRepository.SaveChangesAsync();
                }

                // 2. Remove UserGroups (roles/permissions) to prevent FK constraint issues
                var userGroups = (await _userGroupRepository.GetListAsync(ug => ug.UserId == currentUserId)).ToList();
                foreach (var ug in userGroups)
                {
                    _userGroupRepository.Delete(ug);
                }
                if (userGroups.Any())
                {
                    await _userGroupRepository.SaveChangesAsync();
                }

                // 3. Remove TenantUser relations
                var tenantUsers = (await _tenantUserRepository.GetListAsync(tu => tu.UserId == currentUserId)).ToList();
                foreach (var tu in tenantUsers)
                {
                    _tenantUserRepository.Delete(tu);
                }
                if (tenantUsers.Any())
                {
                    await _tenantUserRepository.SaveChangesAsync();
                }

                // 4. Hard delete the User row completely from Users table
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();

                return new SuccessResult("Hesabınız ve tüm kullanıcı verileriniz veritabanından kalıcı olarak silindi.");
            }
        }
    }
}
