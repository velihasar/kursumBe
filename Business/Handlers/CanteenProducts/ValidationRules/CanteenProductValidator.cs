using Business.Handlers.CanteenProducts.Commands;
using FluentValidation;

namespace Business.Handlers.CanteenProducts.ValidationRules
{
    public class CreateCanteenProductValidator : AbstractValidator<CreateCanteenProductCommand>
    {
        public CreateCanteenProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ürün adı boş bırakılamaz.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Kategori seçiniz.");
        }
    }

    public class UpdateCanteenProductValidator : AbstractValidator<UpdateCanteenProductCommand>
    {
        public UpdateCanteenProductValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Geçerli bir ürün seçiniz.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ürün adı boş bırakılamaz.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Kategori seçiniz.");
        }
    }
}
