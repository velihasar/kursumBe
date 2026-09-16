using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.CourseEnrollments.ValidationRules;
using Core.Entities.Dtos.CourseEnrollmentDto;
using Core.Extensions;

namespace Business.Handlers.CourseEnrollments.Commands
{
    public class UpdateCourseEnrollmentCommand : IRequest<IDataResult<CourseEnrollmentUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public System.DateTime EnrollmentDate { get; set; }
        public decimal? CustomMonthlyFee { get; set; }
        public int DueDayOfMonth { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }

        public class UpdateCourseEnrollmentCommandHandler : IRequestHandler<UpdateCourseEnrollmentCommand, IDataResult<CourseEnrollmentUpdateResponseDto>>
        {
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IMediator _mediator;

            public UpdateCourseEnrollmentCommandHandler(ICourseEnrollmentRepository courseEnrollmentRepository, IMediator mediator)
            {
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateCourseEnrollmentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CourseEnrollmentUpdateResponseDto>> Handle(UpdateCourseEnrollmentCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var isThereCourseEnrollmentRecord = await _courseEnrollmentRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (isThereCourseEnrollmentRecord == null)
                    return new ErrorDataResult<CourseEnrollmentUpdateResponseDto>("Kayıt bulunamadı.");

                isThereCourseEnrollmentRecord.StudentId = request.StudentId;
                isThereCourseEnrollmentRecord.CourseId = request.CourseId;
                isThereCourseEnrollmentRecord.EnrollmentDate = request.EnrollmentDate;
                isThereCourseEnrollmentRecord.CustomMonthlyFee = request.CustomMonthlyFee;
                isThereCourseEnrollmentRecord.DueDayOfMonth = request.DueDayOfMonth;
                isThereCourseEnrollmentRecord.Status = request.Status;
                isThereCourseEnrollmentRecord.Notes = request.Notes;
                isThereCourseEnrollmentRecord.UpdatedBy = userId > 0 ? userId : null;
                isThereCourseEnrollmentRecord.UpdatedDate = System.DateTime.Now;

                _courseEnrollmentRepository.Update(isThereCourseEnrollmentRecord);
                await _courseEnrollmentRepository.SaveChangesAsync();

                var dto = new CourseEnrollmentUpdateResponseDto
                {
                    Id = isThereCourseEnrollmentRecord.Id,
                    StudentId = isThereCourseEnrollmentRecord.StudentId,
                    CourseId = isThereCourseEnrollmentRecord.CourseId,
                    EnrollmentDate = isThereCourseEnrollmentRecord.EnrollmentDate,
                    CustomMonthlyFee = isThereCourseEnrollmentRecord.CustomMonthlyFee,
                    DueDayOfMonth = isThereCourseEnrollmentRecord.DueDayOfMonth,
                    Status = isThereCourseEnrollmentRecord.Status,
                    Notes = isThereCourseEnrollmentRecord.Notes
                };

                return new SuccessDataResult<CourseEnrollmentUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
