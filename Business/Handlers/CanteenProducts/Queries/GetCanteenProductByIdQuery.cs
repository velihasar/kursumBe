using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.CanteenProductDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.CanteenProducts.Queries
{
    public class GetCanteenProductByIdQuery : IRequest<IDataResult<CanteenProductGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetCanteenProductByIdQueryHandler : IRequestHandler<GetCanteenProductByIdQuery, IDataResult<CanteenProductGetByIdDto>>
        {
            private readonly ICanteenProductRepository _productRepository;

            public GetCanteenProductByIdQueryHandler(ICanteenProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<CanteenProductGetByIdDto>> Handle(GetCanteenProductByIdQuery request, CancellationToken cancellationToken)
            {
                var product = await _productRepository.Query()
                    .Include(p => p.Tenant)
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false, cancellationToken);

                if (product == null)
                {
                    return new ErrorDataResult<CanteenProductGetByIdDto>("Ürün bulunamadı.");
                }

                var dto = new CanteenProductGetByIdDto
                {
                    Id = product.Id,
                    TenantId = product.TenantId,
                    TenantName = product.Tenant?.Name,
                    Name = product.Name,
                    Price = product.Price,
                    Category = product.Category,
                    Barcode = product.Barcode,
                    StockQuantity = product.StockQuantity,
                    Icon = product.Icon,
                    Description = product.Description,
                    IsActive = product.IsActive
                };

                return new SuccessDataResult<CanteenProductGetByIdDto>(dto);
            }
        }
    }
}
