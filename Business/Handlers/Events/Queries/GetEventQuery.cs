using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.EventDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Events.Queries
{
    public class GetEventQuery : IRequest<IDataResult<EventGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetEventQueryHandler : IRequestHandler<GetEventQuery, IDataResult<EventGetByIdDto>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMediator _mediator;

            public GetEventQueryHandler(IEventRepository eventRepository, IMediator mediator)
            {
                _eventRepository = eventRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<EventGetByIdDto>> Handle(GetEventQuery request, CancellationToken cancellationToken)
            {
                var eventItem = await _eventRepository.Query()
                    .Include(t => t.Tenant)
                    .Include(t => t.Branch)
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false, cancellationToken);

                if (eventItem == null)
                    return new ErrorDataResult<EventGetByIdDto>("Etkinlik bulunamadı.");

                var dto = new EventGetByIdDto
                {
                    Id = eventItem.Id,
                    TenantId = eventItem.TenantId,
                    TenantName = eventItem.Tenant != null ? eventItem.Tenant.Name : null,
                    Title = eventItem.Title,
                    Description = eventItem.Description,
                    Category = eventItem.Category,
                    StartDate = eventItem.StartDate,
                    EndDate = eventItem.EndDate,
                    Location = eventItem.Location,
                    TargetAudience = eventItem.TargetAudience,
                    TargetRole = eventItem.TargetRole,
                    Capacity = eventItem.Capacity,
                    IsRegistrationRequired = eventItem.IsRegistrationRequired,
                    ImageUrl = eventItem.ImageUrl,
                    Icon = eventItem.Icon,
                    Status = eventItem.Status,
                    BranchId = eventItem.BranchId,
                    BranchName = eventItem.Branch != null ? eventItem.Branch.Name : null,
                    IsActive = eventItem.IsActive
                };

                return new SuccessDataResult<EventGetByIdDto>(dto);
            }
        }
    }
}
