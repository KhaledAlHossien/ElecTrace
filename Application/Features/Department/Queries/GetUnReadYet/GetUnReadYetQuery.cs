using Application_Contract.DTOs.Department;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetUnReadYet
{
    public record GetUnReadYetQuery(Months month, int year) : IRequest<IEnumerable<DepartmentResponseDto>>;
}
