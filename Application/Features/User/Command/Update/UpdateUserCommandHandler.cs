using Application.Features.User.Command.Update;
using Application_Contract.DTOs.User;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserService userService, IRoleService roleService, IMapper mapper)
    {
        _userService = userService;
        _roleService = roleService;
        _mapper = mapper;
    }

    public async Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.Id);
        if (user == null) throw new KeyNotFoundException("User not found.");

      
        if (!string.IsNullOrEmpty(request.UserDto.UserName))
        {
            var existingUser = await _userService.GetByUserNameAsync(request.UserDto.UserName);

            if (existingUser != null && existingUser.Id != request.Id)
            {
                throw new Exception("Username is already taken by another user.");
            }
        }

        _mapper.Map(request.UserDto, user);

        if (!string.IsNullOrEmpty(request.UserDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password);
        }

        await _userService.UpdateAsync(user);

        return _mapper.Map<UserResponseDto>(user);
    }
}