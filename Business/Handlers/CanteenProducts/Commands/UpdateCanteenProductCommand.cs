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
    public class UpdateCanteenProductCommand : IRequest<IDataResult<CanteenProductGetAllDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Barcode { get; set; }
        public int? StockQuantity { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public class UpdateCanteenProductCommandHandler : IRequestHandler<UpdateCanteenProductCommand, IDataResult<CanteenProductGetAllDto>>
        {
            private readonly ICanteenProductRepository _productRepository;

            public UpdateCanteenProductCommandHandler(ICanteenProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            [ValidationAspect(typeof(UpdateCanteenProductValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<CanteenProductGetAllDto>> Handle(UpdateCanteenProductCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();

                var product = await _productRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);
                if (product == null)
                {
                    return new ErrorDataResult<CanteenProductGetAllDto>("Ürün bulunamadı.");
                }

                product.Name = request.Name.Trim();
                product.Price = request.Price;
                product.Category = string.IsNullOrWhiteSpace(request.Category) ? product.Category : request.Category.Trim();
                product.Barcode = request.Barcode?.Trim();
                product.StockQuantity = request.StockQuantity;
                product.Icon = string.IsNullOrWhiteSpace(request.Icon) ? product.Icon : request.Icon.Trim();
                product.Description = request.Description?.Trim();
                if (request.IsActive.HasValue)
                {
                    product.IsActive = request.IsActive.Value;
                }
                product.UpdatedBy = userId > 0 ? userId : null;
                product.UpdatedDate = DateTime.Now;

                _productRepository.Update(product);
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

                return new SuccessDataResult<CanteenProductGetAllDto>(dto, "Kantin ürünü güncellendi.");
            }
        }
    }
}
