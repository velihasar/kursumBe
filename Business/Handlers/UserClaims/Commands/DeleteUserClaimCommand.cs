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

namespace Business.Handlers.UserClaims.Commands
{
    public class DeleteUserClaimCommand : IRequest<IResult>
    {
        public int Id { get; set; }


        public class DeleteUserClaimCommandHandler : IRequestHandler<DeleteUserClaimCommand, IResult>
        {
            private readonly IUserClaimRepository _userClaimRepository;

            public DeleteUserClaimCommandHandler(IUserClaimRepository userClaimRepository)
            {
                _userClaimRepository = userClaimRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(DeleteUserClaimCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();

                // 1. SuperAdmin dışındaki hiçkimse kendi yetkilerini silemez/değiştiremez
                if (request.Id == currentUserId && !isSuperAdmin)
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendi yetkilerini değiştiremezler.");
                }

                var entityToDelete = await _userClaimRepository.GetAsync(x => x.UserId == request.Id);
                if (entityToDelete == null)
                {
                    return new ErrorResult("Kullanıcı yetkisi bulunamadı.");
                }

                if (!isSuperAdmin && Business.Helpers.SecurityHelper.IsSuperAdminClaim(entityToDelete.ClaimId))
                {
                    return new ErrorResult("Süper Admin yetkisini sadece bir Süper Admin silebilir.");
                }

                _userClaimRepository.Delete(entityToDelete);
                await _userClaimRepository.SaveChangesAsync();

                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}