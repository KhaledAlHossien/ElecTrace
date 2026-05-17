using Application_Contract.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly DataContext _context;
        public RoleService(DataContext context) => _context = context;

        public async Task<bool> ExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Roles.AnyAsync(r => r.Name == name && r.Id != excludeId);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }
        public async Task<Role?> GetRoleByIdAsync(int id) => await _context.Roles.FindAsync(id);

        public async Task UpdateRoleAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteRoleAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> IsRoleUsedAsync(int roleId)
        {
            return await _context.Users.AnyAsync(u => u.RoleId == roleId);
        }


    }
}