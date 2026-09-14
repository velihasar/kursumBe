
using Business.Handlers.Branches.Commands;
using FluentValidation;

namespace Business.Handlers.Branches.ValidationRules
{

    public class CreateBranchValidator : AbstractValidator<CreateBranchCommand>
    {
        public CreateBranchValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
        }
    }
    public class UpdateBranchValidator : AbstractValidator<UpdateBranchCommand>
    {
        public UpdateBranchValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
        }
    }
}