
using Business.BusinessAspects;
using Business.Handlers.Branches.FilterBranch;
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
using Core.Entities.Dtos.BranchDto;

namespace Business.Handlers.Branches.Queries
{
    public class GetBranchesQuery : IRequest<IDataResult<IEnumerable<BranchGetAllDto>>>
    {
        public int? TenantId { get; set; }

        public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, IDataResult<IEnumerable<BranchGetAllDto>>>
        {
            private readonly IBranchRepository _branchRepository;
            private readonly IMediator _mediator;

            public GetBranchesQueryHandler(IBranchRepository branchRepository, IMediator mediator)
            {
                _branchRepository = branchRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<BranchGetAllDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = Core.Extensions.UserInfoExtensions.GetTenantIdOrZero();
                var query = _branchRepository.Query().Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    query = query.Where(x => x.TenantId == request.TenantId.Value);
                }

                var list = query.Select(x => new BranchGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    Name = x.Name,
                    Address = x.Address,
                    Phone = x.Phone,
                    IsActive = x.IsActive
                }).ToList();

                return new SuccessDataResult<IEnumerable<BranchGetAllDto>>(list);
            }
        }
    }
}