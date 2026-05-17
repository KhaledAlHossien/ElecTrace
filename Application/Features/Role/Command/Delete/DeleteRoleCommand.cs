using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Role.Command.Delete
{
    public record DeleteRoleCommand(int Id) : IRequest<Unit>;
}
