using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.CourseDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Courses.Queries
{
    public class GetCourseQuery : IRequest<IDataResult<CourseGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, IDataResult<CourseGetByIdDto>>
        {
            private readonly ICourseRepository _courseRepository;
            private readonly IMediator _mediator;

            public GetCourseQueryHandler(ICourseRepository courseRepository, IMediator mediator)
            {
                _courseRepository = courseRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CourseGetByIdDto>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _courseRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Teacher).ThenInclude(t => t.Person);

                var course = await query.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false && (userTenantId <= 0 || p.TenantId == userTenantId), cancellationToken);

                if (course == null)
                    return new ErrorDataResult<CourseGetByIdDto>("Kayıt bulunamadı.");

                var dto = new CourseGetByIdDto
                {
                    Id = course.Id,
                    TenantId = course.TenantId,
                    TenantName = course.Tenant != null ? course.Tenant.Name : null,
                    Name = course.Name,
                    Code = course.Code,
                    Description = course.Description,
                    Price = course.Price,
                    FeeType = course.FeeType,
                    Capacity = course.Capacity,
                    DaysOfWeek = course.DaysOfWeek,
                    StartTime = course.StartTime,
                    EndTime = course.EndTime,
                    TeacherId = course.TeacherId,
                    TeacherName = course.Teacher != null && course.Teacher.Person != null ? $"{course.Teacher.Person.FirstName} {course.Teacher.Person.LastName}".Trim() : null,
                    IsActive = course.IsActive
                };

                return new SuccessDataResult<CourseGetByIdDto>(dto);
            }
        }
    }
}
