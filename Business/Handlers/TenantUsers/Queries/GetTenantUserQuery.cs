
using Business.BusinessAspects;
using Business.Handlers.TenantUsers.FilterTenantUser;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TenantUserDto;

namespace Business.Handlers.TenantUsers.Queries
{
    public class GetTenantUserQuery : IRequest<IDataResult<TenantUserGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetTenantUserQueryHandler : IRequestHandler<GetTenantUserQuery, IDataResult<TenantUserGetByIdDto>>
        {
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;

            public GetTenantUserQueryHandler(ITenantUserRepository tenantUserRepository, IMediator mediator)
            {
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantUserGetByIdDto>> Handle(GetTenantUserQuery request, CancellationToken cancellationToken)
            {
                var tenantUser = await _tenantUserRepository.GetAsync(TenantUserFiltersHelper.GetTenantUserQueryFilter(request));
                if (tenantUser == null)
                    return new ErrorDataResult<TenantUserGetByIdDto>("Kayıt bulunamadı.");

                var dto = new TenantUserGetByIdDto
                {
                    Id = tenantUser.Id,
                    UserId = tenantUser.UserId,
                    BranchId = tenantUser.BranchId
                };
                return new SuccessDataResult<TenantUserGetByIdDto>(dto);
            }
        }
    }
}
