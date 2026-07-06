using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Import
{
    public record ImportDepartmentsCommand(Stream FileStream) : IRequest<bool>;

}
