using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Delete
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Unit>
    {
        private readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {

            var role = await _roleService.GetRoleByIdAsync(request.Id);

            if (role == null)
                throw new KeyNotFoundException("Role not found.");

            await _roleService.DeleteRoleAsync(role);
            return Unit.Value;
        }
    }
}
