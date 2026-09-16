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
using Business.Handlers.CourseEnrollments.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.CourseEnrollmentDto;
using Core.Extensions;

namespace Business.Handlers.CourseEnrollments.Commands
{
    public class CreateCourseEnrollmentCommand : IRequest<IDataResult<CourseEnrollmentCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public System.DateTime EnrollmentDate { get; set; }
        public decimal? CustomMonthlyFee { get; set; }
        public int DueDayOfMonth { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }

        public class CreateCourseEnrollmentCommandHandler : IRequestHandler<CreateCourseEnrollmentCommand, IDataResult<CourseEnrollmentCreateResponseDto>>
        {
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IMediator _mediator;

            public CreateCourseEnrollmentCommandHandler(ICourseEnrollmentRepository courseEnrollmentRepository, IMediator mediator)
            {
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateCourseEnrollmentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CourseEnrollmentCreateResponseDto>> Handle(CreateCourseEnrollmentCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<CourseEnrollmentCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var isThereCourseEnrollmentRecord = _courseEnrollmentRepository.Query().Any(u => u.IsDeleted == false && u.TenantId == targetTenantId && u.StudentId == request.StudentId && u.CourseId == request.CourseId);

                if (isThereCourseEnrollmentRecord)
                    return new ErrorDataResult<CourseEnrollmentCreateResponseDto>(Messages.NameAlreadyExist);

                var addedCourseEnrollment = new CourseEnrollment
                {
                    TenantId = targetTenantId,
                    StudentId = request.StudentId,
                    CourseId = request.CourseId,
                    EnrollmentDate = request.EnrollmentDate,
                    CustomMonthlyFee = request.CustomMonthlyFee,
                    DueDayOfMonth = request.DueDayOfMonth,
                    Status = request.Status,
                    Notes = request.Notes,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _courseEnrollmentRepository.Add(addedCourseEnrollment);
                await _courseEnrollmentRepository.SaveChangesAsync();

                var dto = new CourseEnrollmentCreateResponseDto
                {
                    Id = addedCourseEnrollment.Id,
                    StudentId = addedCourseEnrollment.StudentId,
                    CourseId = addedCourseEnrollment.CourseId,
                    EnrollmentDate = addedCourseEnrollment.EnrollmentDate,
                    CustomMonthlyFee = addedCourseEnrollment.CustomMonthlyFee,
                    DueDayOfMonth = addedCourseEnrollment.DueDayOfMonth,
                    Status = addedCourseEnrollment.Status,
                    Notes = addedCourseEnrollment.Notes
                };

                return new SuccessDataResult<CourseEnrollmentCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}