using Application_Contract.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Delete
{
    public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
    {
        private readonly IRoleService _roleService;

        public DeleteRoleValidator(IRoleService roleService)
        {
            _roleService = roleService;

            RuleFor(x => x.Id)
                        .MustAsync(async (roleId, cancellation) =>
                        {
                            var isUsed = await _roleService.IsRoleUsedAsync(roleId);
                            return !isUsed;
                        })
                        .WithMessage("Cannot delete this role because it is currently assigned to one or more users.");
        }
    }
}
