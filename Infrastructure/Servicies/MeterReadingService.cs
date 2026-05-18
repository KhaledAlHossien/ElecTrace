using Application_Contract.Interfaces;
using Domain.Entities;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
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


    }
}
