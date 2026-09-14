
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.TenantUsers.FilterTenantUser;
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
using Business.Handlers.TenantUsers.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.TenantUserDto;
using Core.Extensions;

namespace Business.Handlers.TenantUsers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateTenantUserCommand : IRequest<IDataResult<TenantUserCreateResponseDto>>
    {

        public int UserId { get; set; }
        public int? BranchId { get; set; }


        public class CreateTenantUserCommandHandler : IRequestHandler<CreateTenantUserCommand, IDataResult<TenantUserCreateResponseDto>>
        {
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;
            public CreateTenantUserCommandHandler(ITenantUserRepository tenantUserRepository, IMediator mediator)
            {
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateTenantUserValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantUserCreateResponseDto>> Handle(CreateTenantUserCommand request, CancellationToken cancellationToken)
            {
                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                var isThereTenantUserRecord = _tenantUserRepository.Query().Any(TenantUserFiltersHelper.CreateTenantUserCommandFilter(request));

                if (isThereTenantUserRecord)
                    return new ErrorDataResult<TenantUserCreateResponseDto>(Messages.NameAlreadyExist);

                var addedTenantUser = new TenantUser
                {
                    TenantId = tenantId,
                    UserId = request.UserId,
                    BranchId = request.BranchId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _tenantUserRepository.Add(addedTenantUser);
                await _tenantUserRepository.SaveChangesAsync();

                var dto = new TenantUserCreateResponseDto
                {
                    Id = addedTenantUser.Id,
                    UserId = addedTenantUser.UserId,
                    BranchId = addedTenantUser.BranchId
                };

                return new SuccessDataResult<TenantUserCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}