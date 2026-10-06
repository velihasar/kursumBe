using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Events.Commands
{
    public class DeleteEventCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, IResult>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMediator _mediator;

            public DeleteEventCommandHandler(IEventRepository eventRepository, IMediator mediator)
            {
                _eventRepository = eventRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var eventToDelete = await _eventRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (eventToDelete == null)
                    return new ErrorResult("Etkinlik bulunamadı.");

                eventToDelete.IsDeleted = true;
                eventToDelete.UpdatedBy = userId > 0 ? userId : null;
                eventToDelete.UpdatedDate = System.DateTime.Now;

                _eventRepository.Update(eventToDelete);
                await _eventRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
