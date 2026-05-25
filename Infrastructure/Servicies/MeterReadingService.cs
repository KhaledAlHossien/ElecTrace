using Application_Contract.Interfaces;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace Infrastructure.Servicies
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly DataContext _context;
        private readonly IDepartmentService _departmentService;

        public MeterReadingService(DataContext context, IDepartmentService departmentService)
        {
            _context = context;
            _departmentService = departmentService;
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

                        // تخزين القيم مباشرة
                        PricePerUnit = pricePerUnit,
                        TotalCost = totalCostFromExcel
                    };

                    // هنا لا نحتاج لاستدعاء CalculateTotalCost لأننا أدخلنا TotalCost يدوياً
                    _context.MeterReadings.Add(reading);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
