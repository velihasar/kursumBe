using Business.Handlers.Contact.Commands;
using FluentValidation;

namespace Business.Handlers.Contact.ValidationRules
{
    public class SendContactMessageValidator : AbstractValidator<SendContactMessageCommand>
    {
        public SendContactMessageValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Ad soyad gereklidir.")
                .MaximumLength(200)
                .WithMessage("Ad soyad en fazla 200 karakter olabilir.");

            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("E-posta gereklidir.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi girin.");

            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage("Konu gereklidir.")
                .MaximumLength(200)
                .WithMessage("Konu en fazla 200 karakter olabilir.");

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Mesaj gereklidir.")
                .MaximumLength(8000)
                .WithMessage("Mesaj çok uzun.");
        }
    }
}
