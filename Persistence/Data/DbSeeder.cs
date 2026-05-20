using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BCrypt.Net.BCrypt;


namespace Persistence.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(DataContext context)
        {
            // 1. إعداد SystemInfo
            if (!context.SystemInfos.Any())
            {
                context.SystemInfos.Add(new SystemInfo
                {
                    ElectricityPricePerKwh = 1000
                });
                await context.SaveChangesAsync();
            }

            // 2. التأكد من وجود دور Admin
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role { Name = "Admin" };
                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }

            // 3. إضافة المستخدم "it"
            if (!context.Users.Any(u => u.UserName == "it"))
            {
                // استخدم الاسم المؤهل بالكامل للدالة
                var passwordHash = HashPassword("it@123456");
                var user = new User
                {
                    UserName = "it",
                    Name = "IT Administrator",
                    RoleId = adminRole.Id,
                    PasswordHash = passwordHash
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }
    }
}