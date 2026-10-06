using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;

namespace Business.Handlers.Announcements.Commands
{
    public class DeleteAnnouncementCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteAnnouncementCommandHandler : IRequestHandler<DeleteAnnouncementCommand, IResult>
        {
            private readonly IAnnouncementRepository _announcementRepository;
            private readonly IMediator _mediator;

            public DeleteAnnouncementCommandHandler(IAnnouncementRepository announcementRepository, IMediator mediator)
            {
                _announcementRepository = announcementRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var announcementToDelete = await _announcementRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (announcementToDelete == null)
                    return new ErrorResult("Duyuru bulunamadı.");

                announcementToDelete.IsDeleted = true;
                announcementToDelete.UpdatedBy = userId > 0 ? userId : null;
                announcementToDelete.UpdatedDate = System.DateTime.Now;

                _announcementRepository.Update(announcementToDelete);
                await _announcementRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
