using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.EventDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.Events.ValidationRules;

namespace Business.Handlers.Events.Commands
{
    public class UpdateEventCommand : IRequest<IDataResult<EventUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public string TargetAudience { get; set; }
        public int TargetRole { get; set; }
        public int? Capacity { get; set; }
        public bool IsRegistrationRequired { get; set; }
        public string ImageUrl { get; set; }
        public string Icon { get; set; }
        public int Status { get; set; }
        public int? BranchId { get; set; }

        public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, IDataResult<EventUpdateResponseDto>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMediator _mediator;

            public UpdateEventCommandHandler(IEventRepository eventRepository, IMediator mediator)
            {
                _eventRepository = eventRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateEventValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<EventUpdateResponseDto>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var eventItem = await _eventRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (eventItem == null)
                    return new ErrorDataResult<EventUpdateResponseDto>("Etkinlik bulunamadı.");

                if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    eventItem.TenantId = request.TenantId.Value;
                }

                eventItem.Title = request.Title;
                eventItem.Description = request.Description;
                eventItem.Category = request.Category;
                eventItem.StartDate = request.StartDate;
                eventItem.EndDate = request.EndDate;
                eventItem.Location = request.Location;
                eventItem.TargetAudience = request.TargetAudience;
                eventItem.TargetRole = request.TargetRole;
                eventItem.Capacity = request.Capacity;
                eventItem.IsRegistrationRequired = request.IsRegistrationRequired;
                eventItem.ImageUrl = request.ImageUrl;
                eventItem.Icon = request.Icon;
                eventItem.Status = request.Status;
                eventItem.BranchId = request.BranchId;
                eventItem.UpdatedBy = userId > 0 ? userId : null;
                eventItem.UpdatedDate = System.DateTime.Now;

                _eventRepository.Update(eventItem);
                await _eventRepository.SaveChangesAsync();

                var dto = new EventUpdateResponseDto
                {
                    Id = eventItem.Id,
                    Title = eventItem.Title,
                    Category = eventItem.Category,
                    StartDate = eventItem.StartDate,
                    Location = eventItem.Location
                };

                return new SuccessDataResult<EventUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
