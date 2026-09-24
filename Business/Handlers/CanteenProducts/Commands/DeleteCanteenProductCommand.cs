using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.CanteenProducts.Commands
{
    public class DeleteCanteenProductCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCanteenProductCommandHandler : IRequestHandler<DeleteCanteenProductCommand, IResult>
        {
            private readonly ICanteenProductRepository _productRepository;

            public DeleteCanteenProductCommandHandler(ICanteenProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(DeleteCanteenProductCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();

                var product = await _productRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);
                if (product == null)
                {
                    return new ErrorResult("Ürün bulunamadı.");
                }

                product.IsDeleted = true;
                product.UpdatedBy = userId > 0 ? userId : null;
                product.UpdatedDate = DateTime.Now;

                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                return new SuccessResult("Ürün silindi.");
            }
        }
    }
}
