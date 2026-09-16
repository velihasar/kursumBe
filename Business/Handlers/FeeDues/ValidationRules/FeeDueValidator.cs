
using Business.Handlers.FeeDues.Commands;
using FluentValidation;

namespace Business.Handlers.FeeDues.ValidationRules
{

    public class CreateFeeDueValidator : AbstractValidator<CreateFeeDueCommand>
    {
        public CreateFeeDueValidator()
        {
            RuleFor(x => x.CourseEnrollmentId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Period).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.PaidAmount).NotEmpty();
            RuleFor(x => x.RemainingAmount).NotEmpty();
            RuleFor(x => x.DueDate).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();

        }
    }
    public class UpdateFeeDueValidator : AbstractValidator<UpdateFeeDueCommand>
    {
        public UpdateFeeDueValidator()
        {
            RuleFor(x => x.CourseEnrollmentId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Period).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.PaidAmount).NotEmpty();
            RuleFor(x => x.RemainingAmount).NotEmpty();
            RuleFor(x => x.DueDate).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();

        }
    }
}