using Domain.Entities;
using Domain.Enums;
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

    Task<IEnumerable<MeterReading>> GetByMonthAndYearAsync(Domain.Enums.Months month, int year);
    Task AddAsync(MeterReading meterReading);
    Task UpdateAsync(MeterReading meterReading);
    Task DeleteAsync(MeterReading meterReading);
    Task<bool> ImportReadingsFromExcel(Stream fileStream, Months month, int year);
    Task<bool> ImportCurrentReadingsFromExcel(Stream fileStream, Months month, int year);
    Task<bool> IsRead(int departmentId, Months month, int year);

  }
}
