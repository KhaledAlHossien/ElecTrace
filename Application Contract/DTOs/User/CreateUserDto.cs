using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.User
{
    public record CreateUserDto(string Name, string UserName, string Password, int RoleId);
}
