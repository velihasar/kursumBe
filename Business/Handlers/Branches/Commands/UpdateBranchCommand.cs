
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.Branches.FilterBranch;
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
using Business.Handlers.Branches.ValidationRules;
using Core.Entities.Dtos.BranchDto;
using Core.Extensions;


namespace Business.Handlers.Branches.Commands
{


    public class UpdateBranchCommand : IRequest<IDataResult<BranchUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, IDataResult<BranchUpdateResponseDto>>
        {
            private readonly IBranchRepository _branchRepository;
            private readonly IMediator _mediator;

            public UpdateBranchCommandHandler(IBranchRepository branchRepository, IMediator mediator)
            {
                _branchRepository = branchRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateBranchValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<BranchUpdateResponseDto>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
            {
                var isThereBranchRecord = await _branchRepository.GetAsync(BranchFiltersHelper.UpdateBranchCommandFilter(request));
                if (isThereBranchRecord == null)
                    return new ErrorDataResult<BranchUpdateResponseDto>("Kayıt bulunamadı.");

                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (userTenantId > 0)
                {
                    if (isThereBranchRecord.TenantId != userTenantId)
                    {
                        return new ErrorDataResult<BranchUpdateResponseDto>("Bu şubeyi güncelleme yetkiniz bulunmuyor.");
                    }
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    isThereBranchRecord.TenantId = request.TenantId.Value;
                }

                if (userId > 0)
                {
                    isThereBranchRecord.UpdatedBy = userId;
                }
                isThereBranchRecord.UpdatedDate = System.DateTime.Now;

                var cleanPhone = !string.IsNullOrWhiteSpace(request.Phone) ? request.Phone.Replace(" ", "").Trim() : null;

                isThereBranchRecord.Name = request.Name;
                isThereBranchRecord.Address = request.Address;
                isThereBranchRecord.Phone = cleanPhone;
                isThereBranchRecord.IsActive = request.IsActive;


                _branchRepository.Update(isThereBranchRecord);
                await _branchRepository.SaveChangesAsync();

                var dto = new BranchUpdateResponseDto
                {
                    Id = isThereBranchRecord.Id,
                    Name = isThereBranchRecord.Name,
                    Address = isThereBranchRecord.Address,
                    Phone = isThereBranchRecord.Phone
                };

                return new SuccessDataResult<BranchUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
