using Application_Contract.Interfaces;
using Domain.Entities;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Servicies
{
    public class SystemInfoService : ISystemInfoService
    {
        private readonly DataContext _context;

        public SystemInfoService(DataContext context)
        {
            _context = context;
        }

        public async Task<SystemInfo?> GetByIdAsync(int id)
        {
            return await _context.SystemInfos
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SystemInfo>> GetAllAsync()
        {
            return await _context.SystemInfos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(SystemInfo systemInfo)
        {
            _context.SystemInfos.Update(systemInfo);
            await _context.SaveChangesAsync();
        }
    }
}
