using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Courses.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.CourseDto;
using Core.Extensions;

namespace Business.Handlers.Courses.Commands
{
    public class CreateCourseCommand : IRequest<IDataResult<CourseCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int FeeType { get; set; }
        public int? Capacity { get; set; }
        public string? DaysOfWeek { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? TeacherId { get; set; }
        public int? BranchId { get; set; }

        public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, IDataResult<CourseCreateResponseDto>>
        {
            private readonly ICourseRepository _courseRepository;
            private readonly ITenantRepository _tenantRepository;
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;

            public CreateCourseCommandHandler(
                ICourseRepository courseRepository,
                ITenantRepository tenantRepository,
                ITenantUserRepository tenantUserRepository,
                IMediator mediator)
            {
                _courseRepository = courseRepository;
                _tenantRepository = tenantRepository;
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateCourseValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CourseCreateResponseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0 && userId > 0)
                {
                    var tenantUser = await _tenantUserRepository.GetAsync(tu => tu.UserId == userId && tu.IsDeleted == false);
                    if (tenantUser != null && tenantUser.TenantId > 0)
                    {
                        targetTenantId = tenantUser.TenantId;
                    }
                    else
                    {
                        var tenant = await _tenantRepository.GetAsync(t => t.CreatedBy == userId && t.IsDeleted == false);
                        if (tenant != null && tenant.Id > 0)
                        {
                            targetTenantId = tenant.Id;
                        }
                        else
                        {
                            var firstTenant = _tenantRepository.Query().FirstOrDefault(t => t.IsDeleted == false && t.IsActive == true);
                            if (firstTenant != null)
                            {
                                targetTenantId = firstTenant.Id;
                            }
                        }
                    }
                }

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<CourseCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul/kurs) seçiniz.");
                }

                var isThereCourseRecord = _courseRepository.Query().Any(u => u.IsDeleted == false && u.TenantId == targetTenantId && u.Name == request.Name);

                if (isThereCourseRecord)
                    return new ErrorDataResult<CourseCreateResponseDto>(Messages.NameAlreadyExist);

                var addedCourse = new Course
                {
                    TenantId = targetTenantId,
                    Name = request.Name,
                    Code = request.Code,
                    Description = request.Description,
                    Price = request.Price,
                    FeeType = request.FeeType,
                    Capacity = request.Capacity,
                    DaysOfWeek = request.DaysOfWeek,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    TeacherId = request.TeacherId,
                    BranchId = request.BranchId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _courseRepository.Add(addedCourse);
                await _courseRepository.SaveChangesAsync();

                var dto = new CourseCreateResponseDto
                {
                    Id = addedCourse.Id,
                    Name = addedCourse.Name,
                    Code = addedCourse.Code,
                    Description = addedCourse.Description,
                    Price = addedCourse.Price,
                    FeeType = addedCourse.FeeType,
                    Capacity = addedCourse.Capacity,
                    DaysOfWeek = addedCourse.DaysOfWeek,
                    StartTime = addedCourse.StartTime,
                    EndTime = addedCourse.EndTime,
                    TeacherId = addedCourse.TeacherId,
                    BranchId = addedCourse.BranchId
                };

                return new SuccessDataResult<CourseCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}