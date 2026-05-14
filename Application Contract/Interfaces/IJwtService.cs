using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(User user);
        Task<bool> RevokeToken(string token);
    }
}
