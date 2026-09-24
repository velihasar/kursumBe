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
using Core.Entities.Dtos.FeeDueDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.FeeDues.Queries
{
    public class GetFeeDuesQuery : IRequest<IDataResult<IEnumerable<FeeDueGetAllDto>>>
    {
        public int? TenantId { get; set; }

        public class GetFeeDuesQueryHandler : IRequestHandler<GetFeeDuesQuery, IDataResult<IEnumerable<FeeDueGetAllDto>>>
        {
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public GetFeeDuesQueryHandler(IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<FeeDueGetAllDto>>> Handle(GetFeeDuesQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _feeDueRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.CourseEnrollment).ThenInclude(ce => ce.Course)
                    .Include(x => x.Student).ThenInclude(s => s.Person)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    query = query.Where(x => x.TenantId == request.TenantId.Value);
                }

                var list = await query.ToListAsync(cancellationToken);
                var dtos = list.Select(x => new FeeDueGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    CourseEnrollmentId = x.CourseEnrollmentId,
                    CourseName = x.CourseEnrollment != null && x.CourseEnrollment.Course != null ? x.CourseEnrollment.Course.Name : null,
                    StudentId = x.StudentId,
                    StudentName = x.Student != null && x.Student.Person != null ? $"{x.Student.Person.FirstName} {x.Student.Person.LastName}".Trim() : null,
                    Period = x.Period,
                    Title = x.Title,
                    Amount = x.Amount,
                    PaidAmount = x.PaidAmount,
                    RemainingAmount = x.RemainingAmount,
                    DueDate = x.DueDate,
                    Status = x.Status,
                    Description = x.Description,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<FeeDueGetAllDto>>(dtos);
            }
        }
    }
}