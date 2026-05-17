using Application_Contract.DTOs.Role;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using RoleEntity = Domain.Entities.Role;

namespace Application.Features.Role.Command.Create
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleResponseDto>
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<RoleResponseDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new RoleEntity { Name = request.RoleDto.Name };

            await _roleService.AddRoleAsync(role);

            return _mapper.Map<RoleResponseDto>(role);
        }
    }
}
