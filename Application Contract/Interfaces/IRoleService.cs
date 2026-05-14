using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> GetByIdAsync(int id);
    }
}
