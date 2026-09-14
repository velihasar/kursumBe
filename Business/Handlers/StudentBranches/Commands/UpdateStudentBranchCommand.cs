using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.StudentBranches.FilterStudentBranch;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentBranchDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.StudentBranches.ValidationRules;

namespace Business.Handlers.StudentBranches.Commands
{
    public class UpdateStudentBranchCommand : IRequest<IDataResult<StudentBranchUpdateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int StudentId { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; } = true;

        public class UpdateStudentBranchCommandHandler : IRequestHandler<UpdateStudentBranchCommand, IDataResult<StudentBranchUpdateResponseDto>>
        {
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IMediator _mediator;

            public UpdateStudentBranchCommandHandler(IStudentBranchRepository studentBranchRepository, IMediator mediator)
            {
                _studentBranchRepository = studentBranchRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateStudentBranchValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentBranchUpdateResponseDto>> Handle(UpdateStudentBranchCommand request, CancellationToken cancellationToken)
            {
                var isThereRecord = await _studentBranchRepository.GetAsync(StudentBranchFiltersHelper.UpdateStudentBranchCommandFilter(request));
                if (isThereRecord == null)
                {
                    return new ErrorDataResult<StudentBranchUpdateResponseDto>("Kayıt bulunamadı.");
                }

                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                if (tenantId > 0)
                {
                    isThereRecord.TenantId = tenantId;
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    isThereRecord.TenantId = request.TenantId.Value;
                }

                if (userId > 0)
                {
                    isThereRecord.UpdatedBy = userId;
                }
                isThereRecord.UpdatedDate = System.DateTime.Now;
                isThereRecord.IsActive = request.IsActive;

                _studentBranchRepository.Update(isThereRecord);
                await _studentBranchRepository.SaveChangesAsync();

                var dto = new StudentBranchUpdateResponseDto
                {
                    StudentId = isThereRecord.StudentId,
                    BranchId = isThereRecord.BranchId,
                    TenantId = isThereRecord.TenantId
                };

                return new SuccessDataResult<StudentBranchUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
