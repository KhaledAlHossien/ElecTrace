using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.User
{
    public record UserDto(int Id, string name, string UserName, string RoleName);
}
