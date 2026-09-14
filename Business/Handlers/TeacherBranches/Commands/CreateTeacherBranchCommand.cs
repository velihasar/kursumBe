using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.TeacherBranches.FilterTeacherBranch;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.TeacherBranchDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.TeacherBranches.ValidationRules;

namespace Business.Handlers.TeacherBranches.Commands
{
    public class CreateTeacherBranchCommand : IRequest<IDataResult<TeacherBranchCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int TeacherId { get; set; }
        public int BranchId { get; set; }

        public class CreateTeacherBranchCommandHandler : IRequestHandler<CreateTeacherBranchCommand, IDataResult<TeacherBranchCreateResponseDto>>
        {
            private readonly ITeacherBranchRepository _teacherBranchRepository;
            private readonly IMediator _mediator;

            public CreateTeacherBranchCommandHandler(ITeacherBranchRepository teacherBranchRepository, IMediator mediator)
            {
                _teacherBranchRepository = teacherBranchRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateTeacherBranchValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TeacherBranchCreateResponseDto>> Handle(CreateTeacherBranchCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<TeacherBranchCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var isThereRecord = _teacherBranchRepository.Query().Any(TeacherBranchFiltersHelper.CreateTeacherBranchCommandFilter(request));
                if (isThereRecord)
                {
                    return new ErrorDataResult<TeacherBranchCreateResponseDto>(Messages.NameAlreadyExist);
                }

                var addedTeacherBranch = new TeacherBranch
                {
                    TenantId = targetTenantId,
                    TeacherId = request.TeacherId,
                    BranchId = request.BranchId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _teacherBranchRepository.Add(addedTeacherBranch);
                await _teacherBranchRepository.SaveChangesAsync();

                var dto = new TeacherBranchCreateResponseDto
                {
                    TeacherId = addedTeacherBranch.TeacherId,
                    BranchId = addedTeacherBranch.BranchId,
                    TenantId = addedTeacherBranch.TenantId
                };

                return new SuccessDataResult<TeacherBranchCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}