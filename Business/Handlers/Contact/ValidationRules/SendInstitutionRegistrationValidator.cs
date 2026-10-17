using Business.Handlers.Contact.Commands;
using FluentValidation;

namespace Business.Handlers.Contact.ValidationRules
{
    public class SendInstitutionRegistrationValidator : AbstractValidator<SendInstitutionRegistrationCommand>
    {
        public SendInstitutionRegistrationValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Yetkili ad soyad gereklidir.")
                .MaximumLength(150)
                .WithMessage("Ad soyad en fazla 150 karakter olabilir.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Telefon numarası gereklidir.")
                .MaximumLength(30)
                .WithMessage("Telefon numarası çok uzun.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-posta adresi gereklidir.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.InstitutionName)
                .NotEmpty()
                .WithMessage("Kurum adı gereklidir.")
                .MaximumLength(200)
                .WithMessage("Kurum adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.City)
                .MaximumLength(100)
                .WithMessage("Şehir alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Message)
                .MaximumLength(4000)
                .WithMessage("Mesaj en fazla 4000 karakter olabilir.");
        }
    }
}
