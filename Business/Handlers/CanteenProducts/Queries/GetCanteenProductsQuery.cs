using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.CanteenProductDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.CanteenProducts.Queries
{
    public class GetCanteenProductsQuery : IRequest<IDataResult<IEnumerable<CanteenProductGetAllDto>>>
    {
        public int? TenantId { get; set; }
        public string Category { get; set; }
        public string SearchTerm { get; set; }

        public class GetCanteenProductsQueryHandler : IRequestHandler<GetCanteenProductsQuery, IDataResult<IEnumerable<CanteenProductGetAllDto>>>
        {
            private readonly ICanteenProductRepository _productRepository;

            public GetCanteenProductsQueryHandler(ICanteenProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<IEnumerable<CanteenProductGetAllDto>>> Handle(GetCanteenProductsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var query = _productRepository.Query()
                    .Include(p => p.Tenant)
                    .Where(p => p.IsDeleted == false);

                if (targetTenantId > 0)
                {
                    query = query.Where(p => p.TenantId == targetTenantId);
                }

                if (!string.IsNullOrWhiteSpace(request.Category))
                {
                    query = query.Where(p => p.Category == request.Category);
                }

                var list = await query.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync(cancellationToken);

                var dtos = list.Select(p => new CanteenProductGetAllDto
                {
                    Id = p.Id,
                    TenantId = p.TenantId,
                    TenantName = p.Tenant != null ? p.Tenant.Name : null,
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.Category,
                    Barcode = p.Barcode,
                    StockQuantity = p.StockQuantity,
                    Icon = p.Icon,
                    Description = p.Description,
                    IsActive = p.IsActive
                });

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.Trim().ToLower();
                    dtos = dtos.Where(p => (p.Name != null && p.Name.ToLower().Contains(term)) ||
                                           (p.Barcode != null && p.Barcode.ToLower().Contains(term)) ||
                                           (p.Category != null && p.Category.ToLower().Contains(term)));
                }

                return new SuccessDataResult<IEnumerable<CanteenProductGetAllDto>>(dtos);
            }
        }
    }
}
