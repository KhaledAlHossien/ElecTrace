using Application_Contract.DTOs.User;
using Application_Contract.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Servicies
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext _context)
        {
            this._context = _context;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task<List<User>> SearchByNameAsync(string name)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Name.Contains(name)) 
                .ToListAsync();
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking() 
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task<string?> GetUserNameByIdAsync(string userId)
        {
            
            if (int.TryParse(userId, out int id))
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                return user?.Name;
            }

            return "غير معروف";
        }
    }
}
