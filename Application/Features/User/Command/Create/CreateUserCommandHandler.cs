using Application.Features.User.Command.Create;
using Application_Contract.DTOs;
using Application_Contract.DTOs.User;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.User.Command.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userService.GetByUserNameAsync(request.UserData.UserName);
            if (existingUser != null)
            {
                throw new Exception("Username already exists, please choose another one.");
            }

            var user = _mapper.Map<Domain.Entities.User>(request.UserData);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.UserData.Password);

            await _userService.AddAsync(user);

            var fullUser = await _userService.GetByIdAsync(user.Id);

            return _mapper.Map<UserDto>(fullUser);
        }
    }
}