using Business.Handlers.Announcements.Commands;
using FluentValidation;

namespace Business.Handlers.Announcements.ValidationRules
{
    public class CreateAnnouncementValidator : AbstractValidator<CreateAnnouncementCommand>
    {
        public CreateAnnouncementValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Başlık zorunludur.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("İçerik zorunludur.");
        }
    }

    public class UpdateAnnouncementValidator : AbstractValidator<UpdateAnnouncementCommand>
    {
        public UpdateAnnouncementValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().WithMessage("Başlık zorunludur.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("İçerik zorunludur.");
        }
    }
}