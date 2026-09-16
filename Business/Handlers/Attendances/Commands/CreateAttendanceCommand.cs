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
using Business.Handlers.Attendances.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.AttendanceDto;
using Core.Extensions;

namespace Business.Handlers.Attendances.Commands
{
    public class CreateAttendanceCommand : IRequest<IDataResult<AttendanceCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public System.DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public string Reason { get; set; }

        public class CreateAttendanceCommandHandler : IRequestHandler<CreateAttendanceCommand, IDataResult<AttendanceCreateResponseDto>>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public CreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateAttendanceValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AttendanceCreateResponseDto>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<AttendanceCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var isThereAttendanceRecord = _attendanceRepository.Query().Any(u => u.IsDeleted == false && u.TenantId == targetTenantId && u.CourseId == request.CourseId && u.StudentId == request.StudentId && u.AttendanceDate == request.AttendanceDate);

                if (isThereAttendanceRecord)
                    return new ErrorDataResult<AttendanceCreateResponseDto>(Messages.NameAlreadyExist);

                var addedAttendance = new Attendance
                {
                    TenantId = targetTenantId,
                    CourseId = request.CourseId,
                    StudentId = request.StudentId,
                    AttendanceDate = request.AttendanceDate,
                    IsPresent = request.IsPresent,
                    Reason = request.Reason,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _attendanceRepository.Add(addedAttendance);
                await _attendanceRepository.SaveChangesAsync();

                var dto = new AttendanceCreateResponseDto
                {
                    Id = addedAttendance.Id,
                    CourseId = addedAttendance.CourseId,
                    StudentId = addedAttendance.StudentId,
                    AttendanceDate = addedAttendance.AttendanceDate,
                    IsPresent = addedAttendance.IsPresent,
                    Reason = addedAttendance.Reason
                };

                return new SuccessDataResult<AttendanceCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}