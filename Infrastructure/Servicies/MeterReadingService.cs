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

        public MeterReadingService(DataContext context)
        {
            _context = context;
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

                    if (string.IsNullOrWhiteSpace(cellA) ||
                        cellA.Contains("الرقم") ||
                        cellA.Contains("Page") ||
                        !int.TryParse(cellA, out _))
                    {
                        continue;
                    }

                    decimal.TryParse(row.Cell("C").GetString(), out decimal prevReading);
                    decimal.TryParse(row.Cell("D").GetString(), out decimal currReading);
                    decimal.TryParse(row.Cell("E").GetString(), out decimal actualCons);

                    // ⚠️ التعديل هنا: قراءة السعر من عمود معين في الإكسل (مثلاً العمود G)
                    // تأكد من تغيير حرف العمود 'G' ليتناسب مع موقع سعر الكيلو في ملف الإكسل عندك
                    decimal.TryParse(row.Cell("G").GetString(), out decimal priceFromExcel);

                    var reading = new MeterReading
                    {
                        DepartmentId = 1, // أو منطق جلب الـ ID
                        DepartmentName = row.Cell("B").GetString(),
                        Month = month,
                        PreviousReading = prevReading,
                        CurrentReading = currReading,
                        ActualConsumption = actualCons,
                        CreatedAt = entryDate
                    };

                    // نمرر السعر الذي قرأناه من الإكسل للميثود
                    // الميثود ستقوم بتخزين هذا السعر في PricePerUnit وحساب التكلفة بناءً عليه
                    reading.CalculateTotalCost(priceFromExcel, 0);

                    _context.MeterReadings.Add(reading);
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
