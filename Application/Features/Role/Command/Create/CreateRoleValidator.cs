using Application_Contract.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Create
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        private readonly IRoleService _roleService;

        public CreateRoleValidator(IRoleService roleService)
        {
            _roleService = roleService;

            RuleFor(x => x.RoleDto.Name)
                .NotEmpty().WithMessage("Role name is required")
                .MinimumLength(2).WithMessage("Role name is too short")
                .MustAsync(async (name, cancellation) =>
                {
                    return !await _roleService.ExistsAsync(name);
                }).WithMessage("This role name already exists.");

        }
    }
}
