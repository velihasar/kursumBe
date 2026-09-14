
using Business.Handlers.Teachers.Commands;
using FluentValidation;

namespace Business.Handlers.Teachers.ValidationRules
{

    public class CreateTeacherValidator : AbstractValidator<CreateTeacherCommand>
    {
        public CreateTeacherValidator()
        {
            RuleFor(x => x.PersonId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
        }
    }
    public class UpdateTeacherValidator : AbstractValidator<UpdateTeacherCommand>
    {
        public UpdateTeacherValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PersonId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
        }
    }
}