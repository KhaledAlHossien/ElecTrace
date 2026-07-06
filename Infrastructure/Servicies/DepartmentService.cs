using Application_Contract.Interfaces;
using Domain.Entities;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;
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
    }
}
