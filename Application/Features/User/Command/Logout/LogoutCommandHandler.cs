using Application_Contract.Interfaces; // تأكد من استيراد المسار الصحيح للواجهة
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.User.Command.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IJwtService _jwtService;

        public LogoutCommandHandler(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token) || !request.Token.StartsWith("Bearer "))
            {
                throw new ArgumentException("Invalid or missing Bearer token.");
            }

            var cleanToken = request.Token.Replace("Bearer ", "");

            var result = await _jwtService.RevokeToken(cleanToken);

            return result;
        }
    }
}