using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.User
{
    public record UserResponseDto(int Id, string name, string UserName, string RoleName);
}
