using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
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
    public class CreateEventCommand : IRequest<IDataResult<EventCreateResponseDto>>
    {
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

        public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, IDataResult<EventCreateResponseDto>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMediator _mediator;

            public CreateEventCommandHandler(IEventRepository eventRepository, IMediator mediator)
            {
                _eventRepository = eventRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateEventValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<EventCreateResponseDto>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var addedEvent = new Event
                {
                    TenantId = targetTenantId,
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Location = request.Location,
                    TargetAudience = request.TargetAudience,
                    TargetRole = request.TargetRole,
                    Capacity = request.Capacity,
                    IsRegistrationRequired = request.IsRegistrationRequired,
                    ImageUrl = request.ImageUrl,
                    Icon = request.Icon,
                    Status = request.Status,
                    BranchId = request.BranchId,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };

                _eventRepository.Add(addedEvent);
                await _eventRepository.SaveChangesAsync();

                var dto = new EventCreateResponseDto
                {
                    Id = addedEvent.Id,
                    Title = addedEvent.Title,
                    Category = addedEvent.Category,
                    StartDate = addedEvent.StartDate,
                    Location = addedEvent.Location
                };

                return new SuccessDataResult<EventCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}