using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.User
{
    public record UpdateUserRequestDto(string? Name, string? UserName, string? Password);
}
