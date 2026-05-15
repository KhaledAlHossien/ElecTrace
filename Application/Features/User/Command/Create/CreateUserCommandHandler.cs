using Application.Features.User.Command.Create;
using Application_Contract.DTOs.User;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.User.Command.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService; // إضافة خدمة الرولز
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserService userService, IRoleService roleService, IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetByIdAsync(request.UserData.RoleId);
            if (role == null)
            {
                throw new Exception("The specified Role ID does not exist in the system.");
            }

            var existingUser = await _userService.GetByUserNameAsync(request.UserData.UserName);
            if (existingUser != null)
            {
                throw new Exception("Username already exists, please choose another one.");
            }

            var user = _mapper.Map<Domain.Entities.User>(request.UserData);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.UserData.Password);

            await _userService.AddAsync(user);

            var fullUser = await _userService.GetByIdAsync(user.Id);

            return _mapper.Map<UserResponseDto>(fullUser);
        }
    }
}