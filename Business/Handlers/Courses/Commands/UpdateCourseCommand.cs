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
using Business.Handlers.Courses.ValidationRules;
using Core.Entities.Dtos.CourseDto;
using Core.Extensions;

namespace Business.Handlers.Courses.Commands
{
    public class UpdateCourseCommand : IRequest<IDataResult<CourseUpdateResponseDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int FeeType { get; set; }
        public int? Capacity { get; set; }
        public string DaysOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? TeacherId { get; set; }

        public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, IDataResult<CourseUpdateResponseDto>>
        {
            private readonly ICourseRepository _courseRepository;
            private readonly IMediator _mediator;

            public UpdateCourseCommandHandler(ICourseRepository courseRepository, IMediator mediator)
            {
                _courseRepository = courseRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateCourseValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CourseUpdateResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var isThereCourseRecord = await _courseRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (isThereCourseRecord == null)
                    return new ErrorDataResult<CourseUpdateResponseDto>("Kayıt bulunamadı.");

                isThereCourseRecord.Name = request.Name;
                isThereCourseRecord.Code = request.Code;
                isThereCourseRecord.Description = request.Description;
                isThereCourseRecord.Price = request.Price;
                isThereCourseRecord.FeeType = request.FeeType;
                isThereCourseRecord.Capacity = request.Capacity;
                isThereCourseRecord.DaysOfWeek = request.DaysOfWeek;
                isThereCourseRecord.StartTime = request.StartTime;
                isThereCourseRecord.EndTime = request.EndTime;
                isThereCourseRecord.TeacherId = request.TeacherId;
                isThereCourseRecord.UpdatedBy = userId > 0 ? userId : null;
                isThereCourseRecord.UpdatedDate = System.DateTime.Now;

                _courseRepository.Update(isThereCourseRecord);
                await _courseRepository.SaveChangesAsync();

                var dto = new CourseUpdateResponseDto
                {
                    Id = isThereCourseRecord.Id,
                    Name = isThereCourseRecord.Name,
                    Code = isThereCourseRecord.Code,
                    Description = isThereCourseRecord.Description,
                    Price = isThereCourseRecord.Price,
                    FeeType = isThereCourseRecord.FeeType,
                    Capacity = isThereCourseRecord.Capacity,
                    DaysOfWeek = isThereCourseRecord.DaysOfWeek,
                    StartTime = isThereCourseRecord.StartTime,
                    EndTime = isThereCourseRecord.EndTime,
                    TeacherId = isThereCourseRecord.TeacherId
                };

                return new SuccessDataResult<CourseUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
