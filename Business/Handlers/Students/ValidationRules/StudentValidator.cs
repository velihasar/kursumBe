
using Business.Handlers.Students.Commands;
using FluentValidation;

namespace Business.Handlers.Students.ValidationRules
{

    public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.PersonId).NotEmpty();
            RuleFor(x => x.StudentNumber).NotEmpty();
            RuleFor(x => x.EnrollmentDate).NotEmpty();
        }
    }
    public class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PersonId).NotEmpty();
            RuleFor(x => x.StudentNumber).NotEmpty();
            RuleFor(x => x.EnrollmentDate).NotEmpty();
        }
    }
}