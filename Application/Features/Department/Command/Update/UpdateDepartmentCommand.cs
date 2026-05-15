using Application_Contract.DTOs.Department  ;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Update
{
    public record UpdateDepartmentCommand(int Id, UpdateDepartmentRequestDto DeptDto) : IRequest<DepartmentResponseDto>;
}
