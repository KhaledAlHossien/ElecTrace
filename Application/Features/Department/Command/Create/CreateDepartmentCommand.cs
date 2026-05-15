using Application_Contract.DTOs.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Create
{
    public record CreateDepartmentCommand(CreateDepartmentRequestDto DeptDto) : IRequest<DepartmentResponseDto>;
}
