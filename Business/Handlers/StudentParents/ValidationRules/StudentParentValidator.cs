
using Business.Handlers.StudentParents.Commands;
using FluentValidation;

namespace Business.Handlers.StudentParents.ValidationRules
{

    public class CreateStudentParentValidator : AbstractValidator<CreateStudentParentCommand>
    {
        public CreateStudentParentValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.ParentId).NotEmpty();
            RuleFor(x => x.Relationship).NotEmpty();
        }
    }
    public class UpdateStudentParentValidator : AbstractValidator<UpdateStudentParentCommand>
    {
        public UpdateStudentParentValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.ParentId).NotEmpty();
            RuleFor(x => x.Relationship).NotEmpty();
        }
    }
}