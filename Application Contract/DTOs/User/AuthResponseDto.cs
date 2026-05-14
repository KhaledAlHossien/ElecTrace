using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.User
{
    public record AuthResponseDto(string Token, string FullName, string UserName);
}
