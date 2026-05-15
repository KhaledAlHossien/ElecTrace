using Application_Contract.Interfaces;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.User.Command.Create
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IRoleService _roleService;

        public CreateUserCommandValidator(IRoleService roleService)
        {
            _roleService = roleService;

            RuleFor(x => x.UserData.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.UserData.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

            RuleFor(x => x.UserData.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.UserData.RoleId)
                .NotEmpty().WithMessage("Role ID is required.")
                .GreaterThan(0).WithMessage("Role ID is required and must be valid.");
        }
    }
}