
using Business.Handlers.Parents.Commands;
using FluentValidation;

namespace Business.Handlers.Parents.ValidationRules
{

    public class CreateParentValidator : AbstractValidator<CreateParentCommand>
    {
        public CreateParentValidator()
        {
            RuleFor(x => x.PersonId).NotEmpty();
        }
    }
    public class UpdateParentValidator : AbstractValidator<UpdateParentCommand>
    {
        public UpdateParentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PersonId).NotEmpty();
        }
    }
}