using Application_Contract.Interfaces;
using Azure.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
namespace Infrastructure.Servicies
{
  public class MeterReadingService : IMeterReadingService
  {
    private readonly DataContext _context;
    private readonly IDepartmentService _departmentService;
    private readonly ISystemInfoService _systemInfoService;

    public MeterReadingService(DataContext context, IDepartmentService departmentService, ISystemInfoService systemInfoService)
    {
      _context = context;
      _departmentService = departmentService;
      _systemInfoService = systemInfoService;
    }

    public async Task<MeterReading?> GetByIdAsync(int id)
    {
      return await _context.MeterReadings
          .Include(m => m.Department)
          .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MeterReading>> GetAllAsync()
    {
      return await _context.MeterReadings
          .Include(m => m.Department)
          .AsNoTracking()
          .ToListAsync();
    }

    public async Task<IEnumerable<MeterReading>> GetByDepartmentIdAsync(int departmentId)
    {
      return await _context.MeterReadings
          .Where(m => m.DepartmentId == departmentId)
          .OrderByDescending(m => m.CreatedAt)
          .AsNoTracking()
          .Include(m => m.Department)
          .ToListAsync();
    }

    public async Task<IEnumerable<MeterReading>> GetByMonthAndYearAsync(Domain.Enums.Months month, int year)
    {
      return await _context.MeterReadings
          .Where(m => m.Month == month && m.CreatedAt.Year == year)
          .Include(m => m.Department)
          .AsNoTracking()
          .OrderByDescending(m => m.CreatedAt)
          .ToListAsync();
    }

    public async Task AddAsync(MeterReading meterReading)
    {
      await _context.MeterReadings.AddAsync(meterReading);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MeterReading meterReading)
    {
      _context.MeterReadings.Update(meterReading);
      await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(MeterReading meterReading)
    {
      _context.MeterReadings.Remove(meterReading);
      await _context.SaveChangesAsync();
    }




    public async Task<bool> ImportReadingsFromExcel(Stream fileStream, Months month, int year)
    {
      using var workbook = new XLWorkbook(fileStream);
      DateTime entryDate = new DateTime(year, (int)month, 1);

      foreach (var worksheet in workbook.Worksheets)
      {
        var rows = worksheet.RangeUsed()?.RowsUsed();
        if (rows == null) continue;

        foreach (var row in rows)
        {
          if (row.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
            continue;

          string cellA = row.Cell("A").GetString().Trim();

          if (string.IsNullOrWhiteSpace(cellA) || cellA.Contains("الرقم") || cellA.Contains("Page") || !int.TryParse(cellA, out _))
          {
            continue;
          }

          // قراءة القيم من الإكسل
          decimal.TryParse(row.Cell("C").GetString(), out decimal prevReading);
          decimal.TryParse(row.Cell("D").GetString(), out decimal currReading);
          decimal.TryParse(row.Cell("E").GetString(), out decimal actualCons);

          // قراءة السعر والقيمة النهائية من الإكسل
          decimal.TryParse(row.Cell("G").GetString(), out decimal pricePerUnit); // سعر الكيلو
          decimal.TryParse(row.Cell("H").GetString(), out decimal totalCostFromExcel); // القيمة النهائية

          var m_code = row.Cell("I").GetString().Trim();
          var Dep = await _departmentService.GetByMeterCodeAsync(m_code);
          if (Dep == null) { continue; }

          var reading = new MeterReading
          {
            DepartmentId = Dep.Id,
            DepartmentName = row.Cell("B").GetString(),
            Month = month,
            PreviousReading = prevReading,
            CurrentReading = currReading,
            ActualConsumption = actualCons,
            CreatedAt = entryDate,

            PricePerUnit = pricePerUnit,
            TotalCost = totalCostFromExcel
          };

          _context.MeterReadings.Add(reading);
        }
      }
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ImportCurrentReadingsFromExcel(Stream fileStream, Months month, int year)
    {
      using var workbook = new XLWorkbook(fileStream);
      DateTime entryDate = new DateTime(year, (int)month, 1);

      var systemInfo = await _systemInfoService.GetAllAsync();
      var pricePerUnit = systemInfo.FirstOrDefault()?.ElectricityPricePerKwh ?? 0;

      foreach (var worksheet in workbook.Worksheets)
      {
        var rows = worksheet.RangeUsed()?.RowsUsed();
        if (rows == null) continue;

        foreach (var row in rows)
        {
          if (row.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
            continue;

          string cellA = row.Cell("A").GetString().Trim();

          if (string.IsNullOrWhiteSpace(cellA) ||
              cellA.Contains("الرقم") ||
              cellA.Contains("Page") ||
              !int.TryParse(cellA, out _))
          {
            continue;
          }

          var meterCode = row.Cell("B").GetString().Trim();
          if (string.IsNullOrWhiteSpace(meterCode))
            continue;

          var Dep = await _departmentService.GetByMeterCodeAsync(meterCode);
          if (Dep == null) continue;
          if (!Dep.IsActive) continue;
          if (Dep.IsFixed) continue;

          string readingCell = row.Cell("C").GetString().Trim();
          if (!decimal.TryParse(readingCell, out decimal currReading))
            continue;

          var allDepartmentReadings = await GetByDepartmentIdAsync(Dep.Id);
          var lastReading = allDepartmentReadings.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
          var prevReading = lastReading?.CurrentReading ?? 0;

          var reading = new MeterReading
          {
            DepartmentId = Dep.Id,
            DepartmentName = Dep.Name,
            Month = month,
            PreviousReading = prevReading,
            CurrentReading = currReading,
            CreatedAt = entryDate
          };

          reading.CalculateConsumption();
          reading.CalculateTotalCost(pricePerUnit, Dep.Discount, Dep.ConversionFactor);

          _context.MeterReadings.Add(reading);
        }
      }

      return await _context.SaveChangesAsync() > 0;
    }
  }
}
