
using Business.BusinessAspects;
using Business.Handlers.Tenants.FilterTenant;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TenantDto;

namespace Business.Handlers.Tenants.Queries
{
    public class GetTenantQuery : IRequest<IDataResult<TenantGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, IDataResult<TenantGetByIdDto>>
        {
            private readonly ITenantRepository _tenantRepository;
            private readonly IMediator _mediator;

            public GetTenantQueryHandler(ITenantRepository tenantRepository, IMediator mediator)
            {
                _tenantRepository = tenantRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantGetByIdDto>> Handle(GetTenantQuery request, CancellationToken cancellationToken)
            {
                var tenant = await _tenantRepository.GetAsync(TenantFiltersHelper.GetTenantQueryFilter(request));
                if (tenant == null)
                    return new ErrorDataResult<TenantGetByIdDto>("Kayıt bulunamadı.");

                var dto = new TenantGetByIdDto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Code = tenant.Code,
                    LogoUrl = tenant.LogoUrl,
                    IsActive = tenant.IsActive
                };
                return new SuccessDataResult<TenantGetByIdDto>(dto);
            }
        }
    }
}
