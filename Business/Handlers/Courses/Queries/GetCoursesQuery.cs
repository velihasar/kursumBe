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
using Core.Entities.Dtos.CourseDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Courses.Queries
{
    public class GetCoursesQuery : IRequest<IDataResult<IEnumerable<CourseGetAllDto>>>
    {
        public int? TenantId { get; set; }

        public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, IDataResult<IEnumerable<CourseGetAllDto>>>
        {
            private readonly ICourseRepository _courseRepository;
            private readonly IMediator _mediator;

            public GetCoursesQueryHandler(ICourseRepository courseRepository, IMediator mediator)
            {
                _courseRepository = courseRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<CourseGetAllDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _courseRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Teacher).ThenInclude(t => t.Person)
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
                var dtos = list.Select(x => new CourseGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    Name = x.Name,
                    Code = x.Code,
                    Description = x.Description,
                    Price = x.Price,
                    FeeType = x.FeeType,
                    Capacity = x.Capacity,
                    DaysOfWeek = x.DaysOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher != null && x.Teacher.Person != null ? $"{x.Teacher.Person.FirstName} {x.Teacher.Person.LastName}".Trim() : null,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<CourseGetAllDto>>(dtos);
            }
        }
    }
}