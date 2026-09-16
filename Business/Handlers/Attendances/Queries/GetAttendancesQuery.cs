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
using Core.Entities.Dtos.AttendanceDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Attendances.Queries
{
    public class GetAttendancesQuery : IRequest<IDataResult<IEnumerable<AttendanceGetAllDto>>>
    {
        public int? TenantId { get; set; }

        public class GetAttendancesQueryHandler : IRequestHandler<GetAttendancesQuery, IDataResult<IEnumerable<AttendanceGetAllDto>>>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public GetAttendancesQueryHandler(IAttendanceRepository attendanceRepository, IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<AttendanceGetAllDto>>> Handle(GetAttendancesQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _attendanceRepository.Query()
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
                var dtos = list.Select(x => new AttendanceGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    CourseId = x.CourseId,
                    CourseName = x.Course != null ? x.Course.Name : null,
                    StudentId = x.StudentId,
                    StudentName = x.Student != null && x.Student.Person != null ? $"{x.Student.Person.FirstName} {x.Student.Person.LastName}".Trim() : null,
                    AttendanceDate = x.AttendanceDate,
                    IsPresent = x.IsPresent,
                    Reason = x.Reason,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<AttendanceGetAllDto>>(dtos);
            }
        }
    }
}