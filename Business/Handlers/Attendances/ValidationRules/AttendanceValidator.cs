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
        }
    }

    public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceCommand>
    {
        public UpdateAttendanceValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.AttendanceDate).NotEmpty();
        }
    }
}