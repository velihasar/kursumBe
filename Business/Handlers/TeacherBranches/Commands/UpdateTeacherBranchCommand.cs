using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.TeacherBranches.FilterTeacherBranch;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TeacherBranchDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.TeacherBranches.ValidationRules;

namespace Business.Handlers.TeacherBranches.Commands
{
    public class UpdateTeacherBranchCommand : IRequest<IDataResult<TeacherBranchUpdateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int TeacherId { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; } = true;

        public class UpdateTeacherBranchCommandHandler : IRequestHandler<UpdateTeacherBranchCommand, IDataResult<TeacherBranchUpdateResponseDto>>
        {
            private readonly ITeacherBranchRepository _teacherBranchRepository;
            private readonly IMediator _mediator;

            public UpdateTeacherBranchCommandHandler(ITeacherBranchRepository teacherBranchRepository, IMediator mediator)
            {
                _teacherBranchRepository = teacherBranchRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateTeacherBranchValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TeacherBranchUpdateResponseDto>> Handle(UpdateTeacherBranchCommand request, CancellationToken cancellationToken)
            {
                var isThereRecord = await _teacherBranchRepository.GetAsync(TeacherBranchFiltersHelper.UpdateTeacherBranchCommandFilter(request));
                if (isThereRecord == null)
                {
                    return new ErrorDataResult<TeacherBranchUpdateResponseDto>("Kayıt bulunamadı.");
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

                _teacherBranchRepository.Update(isThereRecord);
                await _teacherBranchRepository.SaveChangesAsync();

                var dto = new TeacherBranchUpdateResponseDto
                {
                    TeacherId = isThereRecord.TeacherId,
                    BranchId = isThereRecord.BranchId,
                    TenantId = isThereRecord.TenantId
                };

                return new SuccessDataResult<TeacherBranchUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
