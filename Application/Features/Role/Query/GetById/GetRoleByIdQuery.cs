using Application_Contract.DTOs.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Query.GetById
{
    public record GetRoleByIdQuery(int Id) : IRequest<RoleResponseDto>;
}
