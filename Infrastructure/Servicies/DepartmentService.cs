using Application_Contract.Interfaces;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Servicies
{
  public class DepartmentService : IDepartmentService
  {
    private readonly DataContext _context;
    public DepartmentService(DataContext context)
    {
      _context = context;
    }

    public async Task<Department> CreateAsync(Department dept)
    {
      await _context.Departments.AddAsync(dept);
      await _context.SaveChangesAsync();
      return dept;
    }

    public async Task<Department?> GetByMeterCodeAsync(string meterCode)
    {
      return await _context.Departments
          .AsNoTracking()
          .FirstOrDefaultAsync(d => d.MeterCode == meterCode);
    }
    public async Task<Department?> GetByIdAsync(int id)
    {
      return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task UpdateAsync(Department dept)
    {
      _context.Departments.Update(dept);
      await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var dept = await _context.Departments.FindAsync(id);
      if (dept == null)
        throw new KeyNotFoundException("القسم غير موجود");

      if (await _context.MeterReadings.AnyAsync(m => m.DepartmentId == id))
        throw new InvalidOperationException("لا يمكن حذف الاقسام المرتبطة بقراءات");

      _context.Departments.Remove(dept);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<IEnumerable<Department>> GetByNameAsync(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Take(25)
            .ToListAsync();
      }

      var normalizedName = name.Trim().ToLower();

      return await _context.Departments
          .AsNoTracking()
          .Where(d => d.Name.ToLower().Contains(normalizedName)
              || d.MeterCode.ToLower().Contains(normalizedName))
          .OrderBy(d => d.Name)
          .Take(25)
          .ToListAsync();
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
      return await _context.Departments
          .AsNoTracking()
          .ToListAsync();
    }

    public async Task<string> GenerateMeterCodeAsync()
    {
      var lastCode = await _context.Departments
          .OrderByDescending(d => d.Id)
          .Select(d => d.MeterCode)
          .FirstOrDefaultAsync();

      if (string.IsNullOrWhiteSpace(lastCode))
        return "ETT001";

      var numberPart = int.Parse(lastCode.Substring(3));

      return $"ETT{(numberPart + 1):D3}";
    }


    public async Task<bool> ImportFromExcel(Stream fileStream)
    {
      using var workbook = new XLWorkbook(fileStream);

      // الحصول على آخر رقم مستخدم في MeterCode
      int nextMeterCode = 1;

      var lastCode = await _context.Departments
          .OrderByDescending(d => d.Id)
          .Select(d => d.MeterCode)
          .FirstOrDefaultAsync();

      if (!string.IsNullOrWhiteSpace(lastCode) &&
          lastCode.StartsWith("ETT") &&
          int.TryParse(lastCode.Substring(3), out int lastNumber))
      {
        nextMeterCode = lastNumber + 1;
      }

      foreach (var worksheet in workbook.Worksheets)
      {
        var rows = worksheet.RangeUsed()?.RowsUsed();
        if (rows == null)
          continue;

        foreach (var row in rows)
        {
          if (row.Cells().All(c => string.IsNullOrWhiteSpace(c.GetString())))
            continue;

          decimal.TryParse(row.Cell("B").GetString(), out decimal factor);

          var department = new Department
          {
            Name = row.Cell("A").GetString(),
            ConversionFactor = factor,
            MeterCode = $"ETT{nextMeterCode:D3}",
            Discount = 0,
            IsActive = true,
            IsFixed = false,
            FixedValue = 0
          };

          nextMeterCode++;

          _context.Departments.Add(department);
        }
      }

      return await _context.SaveChangesAsync() > 0;
    }
  }
}
