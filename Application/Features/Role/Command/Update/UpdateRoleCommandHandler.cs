using Application_Contract.DTOs.Role;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Update
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleResponseDto>
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<RoleResponseDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetRoleByIdAsync(request.Id);
            if (role == null)
                throw new KeyNotFoundException("Role not found.");

            role.Name = request.RoleDto.Name;

            await _roleService.UpdateRoleAsync(role);

            return _mapper.Map<RoleResponseDto>(role);
        }
    }
}
