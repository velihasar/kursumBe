
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.Students.FilterStudent;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.Students.ValidationRules;
using Core.Entities.Dtos.StudentDto;
using Core.Extensions;

namespace Business.Handlers.Students.Commands
{
    public class UpdateStudentCommand : IRequest<IDataResult<StudentUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public int PersonId { get; set; }
        public string StudentNumber { get; set; }
        public System.DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }

        public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, IDataResult<StudentUpdateResponseDto>>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly IMediator _mediator;

            public UpdateStudentCommandHandler(IStudentRepository studentRepository, IMediator mediator)
            {
                _studentRepository = studentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateStudentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentUpdateResponseDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
            {
                var isThereStudentRecord = await _studentRepository.GetAsync(StudentFiltersHelper.UpdateStudentCommandFilter(request));
                if (isThereStudentRecord == null)
                    return new ErrorDataResult<StudentUpdateResponseDto>("Kayıt bulunamadı.");

                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (tenantId > 0)
                {
                    isThereStudentRecord.TenantId = tenantId;
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    isThereStudentRecord.TenantId = request.TenantId.Value;
                }
                if (userId > 0)
                {
                    isThereStudentRecord.UpdatedBy = userId;
                }
                isThereStudentRecord.UpdatedDate = System.DateTime.Now;

                isThereStudentRecord.PersonId = request.PersonId;
                isThereStudentRecord.StudentNumber = request.StudentNumber;
                isThereStudentRecord.EnrollmentDate = request.EnrollmentDate;
                isThereStudentRecord.IsActive = request.IsActive;

                _studentRepository.Update(isThereStudentRecord);
                await _studentRepository.SaveChangesAsync();

                var dto = new StudentUpdateResponseDto
                {
                    Id = isThereStudentRecord.Id,
                    PersonId = isThereStudentRecord.PersonId,
                    StudentNumber = isThereStudentRecord.StudentNumber,
                    ParentAccessCode = isThereStudentRecord.ParentAccessCode,
                    EnrollmentDate = isThereStudentRecord.EnrollmentDate
                };

                return new SuccessDataResult<StudentUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}

