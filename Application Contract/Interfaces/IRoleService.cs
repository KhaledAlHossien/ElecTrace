using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface IRoleService
    {
        Task AddRoleAsync(Role role);

        Task UpdateRoleAsync(Role role);

        Task DeleteRoleAsync(Role role);

        Task<List<Role>> GetAllRolesAsync();

        Task<Role?> GetRoleByIdAsync(int id);

        Task<bool> IsRoleUsedAsync(int roleId);

        Task<bool> ExistsAsync(string name, int? excludeId = null);
    }
}
