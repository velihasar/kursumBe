
using Business.Handlers.Attendances.Commands;
using FluentValidation;

namespace Business.Handlers.Attendances.ValidationRules
{

    public class CreateAttendanceValidator : AbstractValidator<CreateAttendanceCommand>
    {
        public CreateAttendanceValidator()
        {
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.AttendanceDate).NotEmpty();
            RuleFor(x => x.IsPresent).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty();

        }
    }
    public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceCommand>
    {
        public UpdateAttendanceValidator()
        {
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.AttendanceDate).NotEmpty();
            RuleFor(x => x.IsPresent).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty();

        }
    }
}