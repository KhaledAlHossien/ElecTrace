using Application_Contract.DTOs.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Create
{
    public record CreateRoleCommand(RoleRequestDto RoleDto) : IRequest<RoleResponseDto> ;
}
