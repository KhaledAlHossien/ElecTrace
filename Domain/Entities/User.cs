using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public required string UserName { get; set; }
        public required int RoleId { get; set; }
        public Role? Role { get; set; }
        public required string PasswordHash { get; set; }


    }
}
