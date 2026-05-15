using Application_Contract.DTOs.User;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Queries.Search
{
    public class GetUserByNameQueryHandler : IRequestHandler<GetUserByNameQuery, List<UserResponseDto>>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetUserByNameQueryHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<List<UserResponseDto>> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
        {
            var users = await _userService.SearchByNameAsync(request.Name);

            if (users == null || users.Count == 0)
            {
                throw new KeyNotFoundException($"No users found with name: {request.Name}");
            }
            return _mapper.Map<List<UserResponseDto>>(users);
        }
    }
}
