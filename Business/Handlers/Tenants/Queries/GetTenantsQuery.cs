
using Business.BusinessAspects;
using Business.Handlers.Tenants.FilterTenant;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Dtos.TenantDto;

namespace Business.Handlers.Tenants.Queries
{
    public class GetTenantsQuery : IRequest<IDataResult<IEnumerable<TenantGetAllDto>>>
    {
        public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, IDataResult<IEnumerable<TenantGetAllDto>>>
        {
            private readonly ITenantRepository _tenantRepository;
            private readonly IMediator _mediator;

            public GetTenantsQueryHandler(ITenantRepository tenantRepository, IMediator mediator)
            {
                _tenantRepository = tenantRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<TenantGetAllDto>>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
            {
                var list = await _tenantRepository.GetListAsync(TenantFiltersHelper.GetTenantsQueryFilter(request));
                var dtos = list.Select(x => new TenantGetAllDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    LogoUrl = x.LogoUrl,
                    IsActive = x.IsActive
                });
                return new SuccessDataResult<IEnumerable<TenantGetAllDto>>(dtos);
            }
        }
    }
}