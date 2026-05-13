using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class UserToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public required int UserId { get; set; }
        public User? User { get; set; }
    }
}
