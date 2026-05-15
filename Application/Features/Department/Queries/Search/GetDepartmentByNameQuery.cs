using Application_Contract.DTOs.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.Search
{
    public record GetDepartmentByNameQuery(string Name) : IRequest<IEnumerable<DepartmentResponseDto>>;
}

