using Business.Handlers.Events.Commands;
using FluentValidation;

namespace Business.Handlers.Events.ValidationRules
{
    public class CreateEventValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Etkinlik başlığı zorunludur.");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Başlangıç tarihi zorunludur.");
        }
    }

    public class UpdateEventValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().WithMessage("Etkinlik başlığı zorunludur.");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Başlangıç tarihi zorunludur.");
        }
    }
}