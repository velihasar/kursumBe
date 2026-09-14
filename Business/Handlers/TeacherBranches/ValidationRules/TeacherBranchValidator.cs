using Business.Handlers.TeacherBranches.Commands;
using FluentValidation;

namespace Business.Handlers.TeacherBranches.ValidationRules
{
    public class CreateTeacherBranchValidator : AbstractValidator<CreateTeacherBranchCommand>
    {
        public CreateTeacherBranchValidator()
        {
            RuleFor(x => x.TeacherId).GreaterThan(0);
            RuleFor(x => x.BranchId).GreaterThan(0);
        }
    }

    public class UpdateTeacherBranchValidator : AbstractValidator<UpdateTeacherBranchCommand>
    {
        public UpdateTeacherBranchValidator()
        {
            RuleFor(x => x.TeacherId).GreaterThan(0);
            RuleFor(x => x.BranchId).GreaterThan(0);
        }
    }
}