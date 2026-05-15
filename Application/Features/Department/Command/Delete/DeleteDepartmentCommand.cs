using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Delete
{
    public record DeleteDepartmentCommand(int Id) : IRequest<bool>;

}
