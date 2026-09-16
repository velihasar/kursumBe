
using Business.Handlers.Payments.Commands;
using FluentValidation;

namespace Business.Handlers.Payments.ValidationRules
{

    public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.PaymentDate).NotEmpty();
            RuleFor(x => x.PaymentType).NotEmpty();
            RuleFor(x => x.ReceiptNo).NotEmpty();
            RuleFor(x => x.TransactionId).NotEmpty();
            RuleFor(x => x.Notes).NotEmpty();

        }
    }
    public class UpdatePaymentValidator : AbstractValidator<UpdatePaymentCommand>
    {
        public UpdatePaymentValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.PaymentDate).NotEmpty();
            RuleFor(x => x.PaymentType).NotEmpty();
            RuleFor(x => x.ReceiptNo).NotEmpty();
            RuleFor(x => x.TransactionId).NotEmpty();
            RuleFor(x => x.Notes).NotEmpty();

        }
    }
}