
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Students.FilterStudent;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Students.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.StudentDto;
using Core.Extensions;

namespace Business.Handlers.Students.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateStudentCommand : IRequest<IDataResult<StudentCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int PersonId { get; set; }
        public string StudentNumber { get; set; }
        public System.DateTime EnrollmentDate { get; set; }


        public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, IDataResult<StudentCreateResponseDto>>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly ITenantRepository _tenantRepository;
            private readonly IMediator _mediator;

            public CreateStudentCommandHandler(IStudentRepository studentRepository, ITenantRepository tenantRepository, IMediator mediator)
            {
                _studentRepository = studentRepository;
                _tenantRepository = tenantRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateStudentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentCreateResponseDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<StudentCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var isThereStudentRecord = _studentRepository.Query().Any(StudentFiltersHelper.CreateStudentCommandFilter(request));

                if (isThereStudentRecord)
                    return new ErrorDataResult<StudentCreateResponseDto>(Messages.NameAlreadyExist);

                var addedStudent = new Student
                {
                    TenantId = targetTenantId,
                    PersonId = request.PersonId,
                    StudentNumber = request.StudentNumber,
                    EnrollmentDate = request.EnrollmentDate,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _studentRepository.Add(addedStudent);
                await _studentRepository.SaveChangesAsync();

                var dto = new StudentCreateResponseDto
                {
                    Id = addedStudent.Id,
                    PersonId = addedStudent.PersonId,
                    StudentNumber = addedStudent.StudentNumber,
                    EnrollmentDate = addedStudent.EnrollmentDate
                };

                return new SuccessDataResult<StudentCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}