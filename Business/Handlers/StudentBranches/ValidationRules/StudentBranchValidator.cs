using Business.Handlers.StudentBranches.Commands;
using FluentValidation;

namespace Business.Handlers.StudentBranches.ValidationRules
{
    public class CreateStudentBranchValidator : AbstractValidator<CreateStudentBranchCommand>
    {
        public CreateStudentBranchValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.BranchId).GreaterThan(0);
        }
    }

    public class UpdateStudentBranchValidator : AbstractValidator<UpdateStudentBranchCommand>
    {
        public UpdateStudentBranchValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.BranchId).GreaterThan(0);
        }
    }
}