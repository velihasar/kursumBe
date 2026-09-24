
using Business.Handlers.Payments.Commands;
using FluentValidation;

namespace Business.Handlers.Payments.ValidationRules
{

    public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.PaymentDate).NotEmpty();
            RuleFor(x => x.PaymentType).GreaterThan(0);
        }
    }
    public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentCommand>
    {
        public UpdatePaymentValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.PaymentDate).NotEmpty();
            RuleFor(x => x.PaymentType).GreaterThan(0);
        }
    }
}