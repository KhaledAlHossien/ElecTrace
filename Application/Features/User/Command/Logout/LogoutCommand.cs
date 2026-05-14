using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Command.Logout
{
    public record LogoutCommand(string Token) : IRequest<bool>;
}
