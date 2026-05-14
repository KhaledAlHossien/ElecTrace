using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Command.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required for logout.");
        }
    }
}
