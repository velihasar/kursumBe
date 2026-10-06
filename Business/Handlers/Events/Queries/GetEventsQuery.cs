using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Dtos.EventDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Events.Queries
{
    public class GetEventsQuery : IRequest<IDataResult<IEnumerable<EventGetAllDto>>>
    {
        public int? TenantId { get; set; }
        public int? BranchId { get; set; }

        public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, IDataResult<IEnumerable<EventGetAllDto>>>
        {
            private readonly IEventRepository _eventRepository;
            private readonly IMediator _mediator;

            public GetEventsQueryHandler(IEventRepository eventRepository, IMediator mediator)
            {
                _eventRepository = eventRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<EventGetAllDto>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _eventRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Branch)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    query = query.Where(x => x.TenantId == request.TenantId.Value);
                }

                if (request.BranchId.HasValue && request.BranchId.Value > 0)
                {
                    query = query.Where(x => x.BranchId == request.BranchId.Value || x.BranchId == null);
                }

                var list = await query.OrderBy(x => x.StartDate).ToListAsync(cancellationToken);
                var dtos = list.Select(x => new EventGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    Title = x.Title,
                    Description = x.Description,
                    Category = x.Category,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Location = x.Location,
                    TargetAudience = x.TargetAudience,
                    TargetRole = x.TargetRole,
                    Capacity = x.Capacity,
                    IsRegistrationRequired = x.IsRegistrationRequired,
                    ImageUrl = x.ImageUrl,
                    Icon = x.Icon,
                    Status = x.Status,
                    BranchId = x.BranchId,
                    BranchName = x.Branch != null ? x.Branch.Name : null,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<EventGetAllDto>>(dtos);
            }
        }
    }
}