
using Business.Handlers.TenantUsers.Commands;
using FluentValidation;

namespace Business.Handlers.TenantUsers.ValidationRules
{

    public class CreateTenantUserValidator : AbstractValidator<CreateTenantUserCommand>
    {
        public CreateTenantUserValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
    public class UpdateTenantUserValidator : AbstractValidator<UpdateTenantUserCommand>
    {
        public UpdateTenantUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}