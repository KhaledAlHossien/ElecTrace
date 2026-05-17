using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface ISystemInfoService
    {
        Task<SystemInfo?> GetByIdAsync(int id);
        Task<IEnumerable<SystemInfo>> GetAllAsync();
        Task UpdateAsync(SystemInfo systemInfo);
    }
}
