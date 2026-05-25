using Application_Contract.DTOs.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByUserNameAsync(string userName);

        Task AddAsync(User user);

        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<List<User>> SearchByNameAsync(string name);
        Task<List<User>> GetAllAsync();
        Task<string?> GetUserNameByIdAsync(string userId);
    }
}
