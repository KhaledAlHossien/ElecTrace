using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface IMeterReadingService
    {
        Task<MeterReading?> GetByIdAsync(int id);
        Task<IEnumerable<MeterReading>> GetAllAsync();
        Task<IEnumerable<MeterReading>> GetByDepartmentIdAsync(int departmentId);

        Task<IEnumerable<MeterReading>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task AddAsync(MeterReading meterReading);
        Task UpdateAsync(MeterReading meterReading);
        Task DeleteAsync(MeterReading meterReading);
    }
}
