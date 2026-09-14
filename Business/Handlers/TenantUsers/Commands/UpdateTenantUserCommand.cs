
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.TenantUsers.FilterTenantUser;
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
using Business.Handlers.TenantUsers.ValidationRules;
using Core.Entities.Dtos.TenantUserDto;
using Core.Extensions;

namespace Business.Handlers.TenantUsers.Commands
{
    public class UpdateTenantUserCommand : IRequest<IDataResult<TenantUserUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BranchId { get; set; }
        public bool IsActive { get; set; }

        public class UpdateTenantUserCommandHandler : IRequestHandler<UpdateTenantUserCommand, IDataResult<TenantUserUpdateResponseDto>>
        {
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;

            public UpdateTenantUserCommandHandler(ITenantUserRepository tenantUserRepository, IMediator mediator)
            {
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateTenantUserValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantUserUpdateResponseDto>> Handle(UpdateTenantUserCommand request, CancellationToken cancellationToken)
            {
                var isThereTenantUserRecord = await _tenantUserRepository.GetAsync(TenantUserFiltersHelper.UpdateTenantUserCommandFilter(request));
                if (isThereTenantUserRecord == null)
                    return new ErrorDataResult<TenantUserUpdateResponseDto>("Kayıt bulunamadı.");

                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (tenantId > 0)
                {
                    isThereTenantUserRecord.TenantId = tenantId;
                }
                if (userId > 0)
                {
                    isThereTenantUserRecord.UpdatedBy = userId;
                }
                isThereTenantUserRecord.UpdatedDate = System.DateTime.Now;

                isThereTenantUserRecord.UserId = request.UserId;
                isThereTenantUserRecord.BranchId = request.BranchId;
                isThereTenantUserRecord.IsActive = request.IsActive;

                _tenantUserRepository.Update(isThereTenantUserRecord);
                await _tenantUserRepository.SaveChangesAsync();

                var dto = new TenantUserUpdateResponseDto
                {
                    Id = isThereTenantUserRecord.Id,
                    UserId = isThereTenantUserRecord.UserId,
                    BranchId = isThereTenantUserRecord.BranchId
                };

                return new SuccessDataResult<TenantUserUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}

