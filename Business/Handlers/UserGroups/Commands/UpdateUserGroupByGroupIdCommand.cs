using System.Linq;
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
    public class UpdateUserGroupByGroupIdCommand : IRequest<IResult>
    {       
        public int GroupId { get; set; }
        public int[] UserIds { get; set; }


        public class UpdateUserGroupByGroupIdCommandHandler : IRequestHandler<UpdateUserGroupByGroupIdCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public UpdateUserGroupByGroupIdCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(UpdateUserGroupByGroupIdCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();
                var userIds = request.UserIds ?? System.Array.Empty<int>();

                // 1. SuperAdmin dışındaki hiçkimse kendine yetki/rol veremez
                if (!isSuperAdmin && userIds.Contains(currentUserId))
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendilerine rol veya yetki atayamazlar.");
                }

                // 2. SuperAdmin dışında kimse kimseye SuperAdmin rolünü veremez
                if (!isSuperAdmin && Business.Helpers.SecurityHelper.IsSuperAdminGroup(request.GroupId))
                {
                    return new ErrorResult("Süper Admin rolünü sadece bir Süper Admin atayabilir.");
                }

                var list = userIds.Select(x => new UserGroup() { GroupId = request.GroupId, UserId = x });
                await _userGroupRepository.BulkInsertByGroupId(request.GroupId, list);
                await _userGroupRepository.SaveChangesAsync();

                return new SuccessResult(Messages.Updated);
            }
        }
    }
}