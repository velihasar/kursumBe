
using Business.BusinessAspects;
using Business.Handlers.TenantUsers.FilterTenantUser;
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
using Core.Entities.Dtos.TenantUserDto;

namespace Business.Handlers.TenantUsers.Queries
{
    public class GetTenantUsersQuery : IRequest<IDataResult<IEnumerable<TenantUserGetAllDto>>>
    {
        public class GetTenantUsersQueryHandler : IRequestHandler<GetTenantUsersQuery, IDataResult<IEnumerable<TenantUserGetAllDto>>>
        {
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;

            public GetTenantUsersQueryHandler(ITenantUserRepository tenantUserRepository, IMediator mediator)
            {
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<TenantUserGetAllDto>>> Handle(GetTenantUsersQuery request, CancellationToken cancellationToken)
            {
                var list = await _tenantUserRepository.GetListAsync(TenantUserFiltersHelper.GetTenantUsersQueryFilter(request));
                var dtos = list.Select(x => new TenantUserGetAllDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    BranchId = x.BranchId
                });
                return new SuccessDataResult<IEnumerable<TenantUserGetAllDto>>(dtos);
            }
        }
    }
}