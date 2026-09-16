using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.AttendanceDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Attendances.Queries
{
    public class GetAttendanceQuery : IRequest<IDataResult<AttendanceGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetAttendanceQueryHandler : IRequestHandler<GetAttendanceQuery, IDataResult<AttendanceGetByIdDto>>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public GetAttendanceQueryHandler(IAttendanceRepository attendanceRepository, IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AttendanceGetByIdDto>> Handle(GetAttendanceQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _attendanceRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Course)
                    .Include(x => x.Student).ThenInclude(s => s.Person);

                var attendance = await query.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false && (userTenantId <= 0 || p.TenantId == userTenantId), cancellationToken);

                if (attendance == null)
                    return new ErrorDataResult<AttendanceGetByIdDto>("Kayıt bulunamadı.");

                var dto = new AttendanceGetByIdDto
                {
                    Id = attendance.Id,
                    TenantId = attendance.TenantId,
                    TenantName = attendance.Tenant != null ? attendance.Tenant.Name : null,
                    CourseId = attendance.CourseId,
                    CourseName = attendance.Course != null ? attendance.Course.Name : null,
                    StudentId = attendance.StudentId,
                    StudentName = attendance.Student != null && attendance.Student.Person != null ? $"{attendance.Student.Person.FirstName} {attendance.Student.Person.LastName}".Trim() : null,
                    AttendanceDate = attendance.AttendanceDate,
                    IsPresent = attendance.IsPresent,
                    Reason = attendance.Reason,
                    IsActive = attendance.IsActive
                };

                return new SuccessDataResult<AttendanceGetByIdDto>(dto);
            }
        }
    }
}
