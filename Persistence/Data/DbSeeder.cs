using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Persistence.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(DataContext context)
        {
            if (!context.SystemInfos.Any())
            {
                context.SystemInfos.AddRange(
                    new SystemInfo
                    {
                        ElectricityPricePerKwh = 1000
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
