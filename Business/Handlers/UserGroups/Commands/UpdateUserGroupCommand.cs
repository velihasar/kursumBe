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
    public class UpdateUserGroupCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int[] GroupId { get; set; }


        public class UpdateUserGroupCommandHandler : IRequestHandler<UpdateUserGroupCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public UpdateUserGroupCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }


            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("GetUsers")]
            [CacheRemoveAspect("GetUserGroups")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(UpdateUserGroupCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();

                // 1. SuperAdmin dışındaki hiçkimse kendine yetki/rol veremez
                if (request.UserId == currentUserId && !isSuperAdmin)
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendi rollerini veya yetkilerini değiştiremezler.");
                }

                var groupIds = request.GroupId ?? System.Array.Empty<int>();

                // 2. SuperAdmin dışında kimse kimseye SuperAdmin rolünü veremez
                if (!isSuperAdmin && Business.Helpers.SecurityHelper.ContainsSuperAdminGroup(groupIds))
                {
                    return new ErrorResult("Süper Admin rolünü sadece bir Süper Admin atayabilir.");
                }

                // 3. SuperAdmin dışında kimse var olan bir SuperAdmin kullanıcısının rollerini değiştiremez
                if (!isSuperAdmin)
                {
                    var existingUserGroups = await _userGroupRepository.GetListAsync(ug => ug.UserId == request.UserId);
                    var existingGroupIds = existingUserGroups.Select(ug => ug.GroupId).ToArray();
                    if (Business.Helpers.SecurityHelper.ContainsSuperAdminGroup(existingGroupIds))
                    {
                        return new ErrorResult("Süper Admin kullanıcısının rollerini sadece bir Süper Admin değiştirebilir.");
                    }
                }

                var userGroupList =
                    groupIds.Select(x => new UserGroup() { GroupId = x, UserId = request.UserId });

                await _userGroupRepository.BulkInsert(request.UserId, userGroupList);
                await _userGroupRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}