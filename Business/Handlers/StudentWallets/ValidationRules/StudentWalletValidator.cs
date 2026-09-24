using Business.Handlers.StudentWallets.Commands;
using FluentValidation;

namespace Business.Handlers.StudentWallets.ValidationRules
{
    public class DepositStudentWalletValidator : AbstractValidator<DepositStudentWalletCommand>
    {
        public DepositStudentWalletValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0).WithMessage("Geçerli bir öğrenci seçiniz.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Yükleme tutarı 0'dan büyük olmalıdır.");
            RuleFor(x => x.PaymentType).InclusiveBetween(1, 5).WithMessage("Geçerli bir ödeme türü seçiniz.");
        }
    }

    public class SpendStudentWalletValidator : AbstractValidator<SpendStudentWalletCommand>
    {
        public SpendStudentWalletValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0).WithMessage("Geçerli bir öğrenci seçiniz.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Harcama tutarı 0'dan büyük olmalıdır.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Kategori (örn. Su, Meşrubat, Kantin) belirtiniz.");
        }
    }
}
