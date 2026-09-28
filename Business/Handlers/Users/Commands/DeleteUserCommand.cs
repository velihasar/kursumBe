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
    public class DeleteUserCommand : IRequest<IResult>
    {
        public int UserId { get; set; }

        public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;

            public DeleteUserCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("GetUsers")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = Core.Extensions.UserInfoExtensions.GetUserIdOrZero();
                var isSuperAdmin = Business.Helpers.SecurityHelper.IsSuperAdmin();

                if (request.UserId == currentUserId && !isSuperAdmin)
                {
                    return new ErrorResult("Süper Admin dışındaki kullanıcılar kendi hesaplarını silemezler.");
                }

                var userToDelete = _userRepository.Get(p => p.UserId == request.UserId);
                if (userToDelete == null)
                {
                    return new ErrorResult("Kullanıcı bulunamadı.");
                }

                userToDelete.Status = false;
                _userRepository.Update(userToDelete);
                await _userRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}