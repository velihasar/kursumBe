using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserClaims.Commands
{
    public class CreateUserClaimCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public int ClaimId { get; set; }

        public class CreateUserClaimCommandHandler : IRequestHandler<CreateUserClaimCommand, IResult>
        {
            private readonly IUserClaimRepository _userClaimRepository;
            private readonly ICacheManager _cacheManager;

            public CreateUserClaimCommandHandler(IUserClaimRepository userClaimRepository, ICacheManager cacheManager)
            {
                _userClaimRepository = userClaimRepository;
                _cacheManager = cacheManager;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(CreateUserClaimCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();

                // 1. SuperAdmin dışındaki hiçkimse kendine yetki veremez
                if (request.UserId == currentUserId && !isSuperAdmin)
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendi yetkilerini değiştiremezler.");
                }

                // 2. SuperAdmin dışında kimse kimseye SuperAdmin yetkisi veremez
                if (!isSuperAdmin && Business.Helpers.SecurityHelper.IsSuperAdminClaim(request.ClaimId))
                {
                    return new ErrorResult("Süper Admin yetkisini sadece bir Süper Admin atayabilir.");
                }

                var userClaim = new UserClaim
                {
                    ClaimId = request.ClaimId,
                    UserId = request.UserId
                };
                _userClaimRepository.Add(userClaim);
                await _userClaimRepository.SaveChangesAsync();

                _cacheManager.Remove($"{CacheKeys.UserIdForClaim}={request.UserId}");

                return new SuccessResult(Messages.Added);
            }
        }
    }
}