using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.CanteenProducts.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.CanteenProductDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.CanteenProducts.Commands
{
    public class CreateCanteenProductCommand : IRequest<IDataResult<CanteenProductGetAllDto>>
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Barcode { get; set; }
        public int? StockQuantity { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }

        public class CreateCanteenProductCommandHandler : IRequestHandler<CreateCanteenProductCommand, IDataResult<CanteenProductGetAllDto>>
        {
            private readonly ICanteenProductRepository _productRepository;

            public CreateCanteenProductCommandHandler(ICanteenProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            [ValidationAspect(typeof(CreateCanteenProductValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<CanteenProductGetAllDto>> Handle(CreateCanteenProductCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<CanteenProductGetAllDto>("Geçerli bir kurum seçiniz.");
                }

                var product = new CanteenProduct
                {
                    TenantId = targetTenantId,
                    Name = request.Name.Trim(),
                    Price = request.Price,
                    Category = string.IsNullOrWhiteSpace(request.Category) ? "Genel" : request.Category.Trim(),
                    Barcode = request.Barcode?.Trim(),
                    StockQuantity = request.StockQuantity,
                    Icon = string.IsNullOrWhiteSpace(request.Icon) ? "🛒" : request.Icon.Trim(),
                    Description = request.Description?.Trim(),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = DateTime.Now
                };

                _productRepository.Add(product);
                await _productRepository.SaveChangesAsync();

                var dto = new CanteenProductGetAllDto
                {
                    Id = product.Id,
                    TenantId = product.TenantId,
                    Name = product.Name,
                    Price = product.Price,
                    Category = product.Category,
                    Barcode = product.Barcode,
                    StockQuantity = product.StockQuantity,
                    Icon = product.Icon,
                    Description = product.Description,
                    IsActive = product.IsActive
                };

                return new SuccessDataResult<CanteenProductGetAllDto>(dto, "Kantin ürünü başarıyla eklendi.");
            }
        }
    }
}
