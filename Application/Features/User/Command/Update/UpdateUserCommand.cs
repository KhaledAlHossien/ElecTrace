    using System;
using System.Collections.Generic;
using System.Text;
using Application_Contract.DTOs.User;   
using MediatR;

namespace Application.Features.User.Command.Update
{
    public record UpdateUserCommand(int Id, UpdateUserRequestDto UserDto) : IRequest<UserResponseDto>;
}
