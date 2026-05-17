using Application_Contract.DTOs.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Query.GetAll
{
    public record GetAllRolesQuery() : IRequest<List<RoleResponseDto>>;
}
