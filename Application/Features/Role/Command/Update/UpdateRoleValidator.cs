using Application_Contract.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Update
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleValidator(IRoleService roleService)
        {
            _roleService = roleService;

            // التحقق من الـ Id
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Role ID is required")
                .GreaterThan(0).WithMessage("Invalid Role ID");

            // التحقق من الاسم داخل الـ Dto
            RuleFor(x => x.RoleDto.Name)
                .NotEmpty().WithMessage("Role name is required")
                .MinimumLength(2).WithMessage("Role name is too short")
                .MustAsync(async (command, name, cancellation) =>
                {
                    bool exists = await _roleService.ExistsAsync(name, command.Id);
                    return !exists;
                })
                .WithMessage("This role name is already taken by another role");
        }
    }
}
