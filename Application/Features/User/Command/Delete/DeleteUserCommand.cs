using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Command.Delete
{
    public record DeleteUserCommand(int Id) : IRequest<bool>;
}
