
using Business.Handlers.FeeDues.Commands;
using FluentValidation;

namespace Business.Handlers.FeeDues.ValidationRules
{

    public class CreateFeeDueValidator : AbstractValidator<CreateFeeDueCommand>
    {
        public CreateFeeDueValidator()
        {
            RuleFor(x => x.CourseEnrollmentId).GreaterThan(0);
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.Period).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.DueDate).NotEmpty();
        }
    }
    public class UpdateFeeDueValidator : AbstractValidator<UpdateFeeDueCommand>
    {
        public UpdateFeeDueValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.CourseEnrollmentId).GreaterThan(0);
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.Period).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.DueDate).NotEmpty();
        }
    }
}