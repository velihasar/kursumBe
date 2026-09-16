
using Business.Handlers.CourseEnrollments.Commands;
using FluentValidation;

namespace Business.Handlers.CourseEnrollments.ValidationRules
{

    public class CreateCourseEnrollmentValidator : AbstractValidator<CreateCourseEnrollmentCommand>
    {
        public CreateCourseEnrollmentValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.EnrollmentDate).NotEmpty();
            RuleFor(x => x.DueDayOfMonth).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.Notes).NotEmpty();

        }
    }
    public class UpdateCourseEnrollmentValidator : AbstractValidator<UpdateCourseEnrollmentCommand>
    {
        public UpdateCourseEnrollmentValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.EnrollmentDate).NotEmpty();
            RuleFor(x => x.DueDayOfMonth).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
            RuleFor(x => x.Notes).NotEmpty();

        }
    }
}