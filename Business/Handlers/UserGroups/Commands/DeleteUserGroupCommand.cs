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

namespace Business.Handlers.UserGroups.Commands
{
    public class DeleteUserGroupCommand : IRequest<IResult>
    {
        public int Id { get; set; }


        public class DeleteUserGroupCommandHandler : IRequestHandler<DeleteUserGroupCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public DeleteUserGroupCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(DeleteUserGroupCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();

                // 1. SuperAdmin dışındaki hiçkimse kendi rollerini silemez/değiştiremez
                if (request.Id == currentUserId && !isSuperAdmin)
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendi rollerini veya yetkilerini değiştiremezler.");
                }

                var entityToDelete = await _userGroupRepository.GetAsync(x => x.UserId == request.Id);
                if (entityToDelete == null)
                {
                    return new ErrorResult("Kullanıcı grubu bulunamadı.");
                }

                if (!isSuperAdmin && Business.Helpers.SecurityHelper.IsSuperAdminGroup(entityToDelete.GroupId))
                {
                    return new ErrorResult("Süper Admin grubunu sadece bir Süper Admin silebilir.");
                }

                _userGroupRepository.Delete(entityToDelete);
                await _userGroupRepository.SaveChangesAsync();

                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}