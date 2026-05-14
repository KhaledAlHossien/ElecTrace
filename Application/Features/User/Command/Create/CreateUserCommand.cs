using Application_Contract.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Command.Create
{
    public record CreateUserCommand(CreateUserDto UserData) : IRequest<UserDto>;
}
