using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.StudentBranches.FilterStudentBranch;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.StudentBranchDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.StudentBranches.ValidationRules;

namespace Business.Handlers.StudentBranches.Commands
{
    public class CreateStudentBranchCommand : IRequest<IDataResult<StudentBranchCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int StudentId { get; set; }
        public int BranchId { get; set; }

        public class CreateStudentBranchCommandHandler : IRequestHandler<CreateStudentBranchCommand, IDataResult<StudentBranchCreateResponseDto>>
        {
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IMediator _mediator;

            public CreateStudentBranchCommandHandler(IStudentBranchRepository studentBranchRepository, IMediator mediator)
            {
                _studentBranchRepository = studentBranchRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateStudentBranchValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentBranchCreateResponseDto>> Handle(CreateStudentBranchCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<StudentBranchCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var isThereRecord = _studentBranchRepository.Query().Any(StudentBranchFiltersHelper.CreateStudentBranchCommandFilter(request));
                if (isThereRecord)
                {
                    return new ErrorDataResult<StudentBranchCreateResponseDto>(Messages.NameAlreadyExist);
                }

                var addedStudentBranch = new StudentBranch
                {
                    TenantId = targetTenantId,
                    StudentId = request.StudentId,
                    BranchId = request.BranchId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _studentBranchRepository.Add(addedStudentBranch);
                await _studentBranchRepository.SaveChangesAsync();

                var dto = new StudentBranchCreateResponseDto
                {
                    StudentId = addedStudentBranch.StudentId,
                    BranchId = addedStudentBranch.BranchId,
                    TenantId = addedStudentBranch.TenantId
                };

                return new SuccessDataResult<StudentBranchCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}