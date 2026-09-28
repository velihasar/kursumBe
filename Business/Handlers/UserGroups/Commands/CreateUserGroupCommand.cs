using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserGroups.Commands
{
    public class CreateUserGroupCommand : IRequest<IResult>
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }

        public class CreateUserGroupCommandHandler : IRequestHandler<CreateUserGroupCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public CreateUserGroupCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();

                // 1. SuperAdmin dışındaki hiçkimse kendine yetki/rol veremez
                if (request.UserId == currentUserId && !isSuperAdmin)
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendi rollerini veya yetkilerini değiştiremezler.");
                }

                // 2. SuperAdmin dışında kimse kimseye SuperAdmin rolünü veremez
                if (!isSuperAdmin && Business.Helpers.SecurityHelper.IsSuperAdminGroup(request.GroupId))
                {
                    return new ErrorResult("Süper Admin rolünü sadece bir Süper Admin atayabilir.");
                }

                var userGroup = new UserGroup
                {
                    GroupId = request.GroupId,
                    UserId = request.UserId
                };

                _userGroupRepository.Add(userGroup);
                await _userGroupRepository.SaveChangesAsync();

                return new SuccessResult(Messages.Added);
            }
        }
    }
}