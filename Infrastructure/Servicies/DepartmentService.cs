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
        public DepartmentService(DataContext context) => _context = context;

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
            if (dept == null) return false;

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Department>> GetByNameAsync(string name)
        {
            return await _context.Departments
                .AsNoTracking()
                .Where(d => d.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}