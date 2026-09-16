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
using Core.Entities.Dtos.CourseEnrollmentDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.CourseEnrollments.Queries
{
    public class GetCourseEnrollmentsQuery : IRequest<IDataResult<IEnumerable<CourseEnrollmentGetAllDto>>>
    {
        public int? TenantId { get; set; }

        public class GetCourseEnrollmentsQueryHandler : IRequestHandler<GetCourseEnrollmentsQuery, IDataResult<IEnumerable<CourseEnrollmentGetAllDto>>>
        {
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IMediator _mediator;

            public GetCourseEnrollmentsQueryHandler(ICourseEnrollmentRepository courseEnrollmentRepository, IMediator mediator)
            {
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CourseEnrollmentGetAllDto>>> Handle(GetCourseEnrollmentsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _courseEnrollmentRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Course)
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
                var dtos = list.Select(x => new CourseEnrollmentGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    StudentId = x.StudentId,
                    StudentName = x.Student != null && x.Student.Person != null ? $"{x.Student.Person.FirstName} {x.Student.Person.LastName}".Trim() : null,
                    CourseId = x.CourseId,
                    CourseName = x.Course != null ? x.Course.Name : null,
                    EnrollmentDate = x.EnrollmentDate,
                    CustomMonthlyFee = x.CustomMonthlyFee,
                    DueDayOfMonth = x.DueDayOfMonth,
                    Status = x.Status,
                    Notes = x.Notes,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<CourseEnrollmentGetAllDto>>(dtos);
            }
        }
    }
}