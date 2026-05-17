using Application_Contract.DTOs.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Update
{
    public record UpdateRoleCommand(int Id, RoleRequestDto RoleDto) : IRequest<RoleResponseDto>;
}
