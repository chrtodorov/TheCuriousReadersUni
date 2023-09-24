using BusinessLayer.Enumerations;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(DataContext dbContext)
        {
            await dbContext.Database.EnsureCreatedAsync();
            if (!await dbContext.Roles.AnyAsync())
            {
                await dbContext.Roles.AddRangeAsync(
                    new RoleEntity { Name = Roles.Administrator },
                    new RoleEntity { Name = Roles.Librarian },
                    new RoleEntity { Name = Roles.Customer });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}