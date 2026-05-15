using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface IDepartmentService
    {
        Task<Department> CreateAsync(Department dept);
        Task<Department?> GetByMeterCodeAsync(string meterCode);
        Task UpdateAsync(Department dept);
        Task<Department?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Department>> GetByNameAsync(string name);
        Task<IEnumerable<Department>> GetAllAsync();    
    }
}
