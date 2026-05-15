using Application_Contract.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Queries.Search
{
    public record GetUserByNameQuery(string Name) : IRequest<List<UserResponseDto>>;
}
