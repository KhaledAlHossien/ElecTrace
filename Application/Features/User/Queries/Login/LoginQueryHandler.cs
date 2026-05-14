using Application_Contract.DTOs;
using Application_Contract.DTOs.User;
using Application_Contract.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.User.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponseDto>
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginQueryHandler> _logger;

        public LoginQueryHandler(
            IUserService userService,
            IJwtService jwtService,
            ILogger<LoginQueryHandler> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {UserName}", request.LoginData.UserName);

                var user = await _userService.GetByUserNameAsync(request.LoginData.UserName);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.LoginData.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for user: {UserName}", request.LoginData.UserName);

                    throw new UnauthorizedAccessException("Invalid username or password.");
                }

             
                var token = await _jwtService.GenerateJwtToken(user);

                _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

                return new AuthResponseDto(
                    Token: token,
                    FullName: user.Name ?? user.UserName,
                    UserName: user.UserName
                );
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login process for user: {UserName}", request.LoginData.UserName);
                throw new Exception("An internal error occurred during the login process.");
            }
        }
    }
}