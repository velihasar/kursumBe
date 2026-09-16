using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.CourseEnrollmentDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.CourseEnrollments.Queries
{
    public class GetCourseEnrollmentQuery : IRequest<IDataResult<CourseEnrollmentGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetCourseEnrollmentQueryHandler : IRequestHandler<GetCourseEnrollmentQuery, IDataResult<CourseEnrollmentGetByIdDto>>
        {
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IMediator _mediator;

            public GetCourseEnrollmentQueryHandler(ICourseEnrollmentRepository courseEnrollmentRepository, IMediator mediator)
            {
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CourseEnrollmentGetByIdDto>> Handle(GetCourseEnrollmentQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _courseEnrollmentRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Course)
                    .Include(x => x.Student).ThenInclude(s => s.Person);

                var courseEnrollment = await query.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false && (userTenantId <= 0 || p.TenantId == userTenantId), cancellationToken);

                if (courseEnrollment == null)
                    return new ErrorDataResult<CourseEnrollmentGetByIdDto>("Kayıt bulunamadı.");

                var dto = new CourseEnrollmentGetByIdDto
                {
                    Id = courseEnrollment.Id,
                    TenantId = courseEnrollment.TenantId,
                    TenantName = courseEnrollment.Tenant != null ? courseEnrollment.Tenant.Name : null,
                    StudentId = courseEnrollment.StudentId,
                    StudentName = courseEnrollment.Student != null && courseEnrollment.Student.Person != null ? $"{courseEnrollment.Student.Person.FirstName} {courseEnrollment.Student.Person.LastName}".Trim() : null,
                    CourseId = courseEnrollment.CourseId,
                    CourseName = courseEnrollment.Course != null ? courseEnrollment.Course.Name : null,
                    EnrollmentDate = courseEnrollment.EnrollmentDate,
                    CustomMonthlyFee = courseEnrollment.CustomMonthlyFee,
                    DueDayOfMonth = courseEnrollment.DueDayOfMonth,
                    Status = courseEnrollment.Status,
                    Notes = courseEnrollment.Notes,
                    IsActive = courseEnrollment.IsActive
                };

                return new SuccessDataResult<CourseEnrollmentGetByIdDto>(dto);
            }
        }
    }
}
