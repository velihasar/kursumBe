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
using Business.Handlers.Attendances.ValidationRules;
using Core.Entities.Dtos.AttendanceDto;
using Core.Extensions;

namespace Business.Handlers.Attendances.Commands
{
    public class UpdateAttendanceCommand : IRequest<IDataResult<AttendanceUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public System.DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public string Reason { get; set; }

        public class UpdateAttendanceCommandHandler : IRequestHandler<UpdateAttendanceCommand, IDataResult<AttendanceUpdateResponseDto>>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public UpdateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateAttendanceValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AttendanceUpdateResponseDto>> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var isThereAttendanceRecord = await _attendanceRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (isThereAttendanceRecord == null)
                    return new ErrorDataResult<AttendanceUpdateResponseDto>("Kayıt bulunamadı.");

                isThereAttendanceRecord.CourseId = request.CourseId;
                isThereAttendanceRecord.StudentId = request.StudentId;
                isThereAttendanceRecord.AttendanceDate = request.AttendanceDate;
                isThereAttendanceRecord.IsPresent = request.IsPresent;
                isThereAttendanceRecord.Reason = request.Reason;
                isThereAttendanceRecord.UpdatedBy = userId > 0 ? userId : null;
                isThereAttendanceRecord.UpdatedDate = System.DateTime.Now;

                _attendanceRepository.Update(isThereAttendanceRecord);
                await _attendanceRepository.SaveChangesAsync();

                var dto = new AttendanceUpdateResponseDto
                {
                    Id = isThereAttendanceRecord.Id,
                    CourseId = isThereAttendanceRecord.CourseId,
                    StudentId = isThereAttendanceRecord.StudentId,
                    AttendanceDate = isThereAttendanceRecord.AttendanceDate,
                    IsPresent = isThereAttendanceRecord.IsPresent,
                    Reason = isThereAttendanceRecord.Reason
                };

                return new SuccessDataResult<AttendanceUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
