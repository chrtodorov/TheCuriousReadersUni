using BusinessLayer.Enumerations;
using BusinessLayer.Models;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAdministratorAsync(DataContext dbContext)
        {
            await dbContext.Database.EnsureCreatedAsync();
            if (!await dbContext.Administrators.AnyAsync() && await dbContext.Roles.AnyAsync())
            {
                var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Administrator);

                if (adminRole is null)
                {
                    return;
                }

                var user = new User
                {
                    FirstName = "Admin",
                    LastName = "1",
                    EmailAddress = "admin@abv.bg",
                    PhoneNumber = "+0123456789",
                    Password = "Administrator1!",
                    RoleName = Roles.Administrator,
                };

                var userEntity = user.ToUserEntity(adminRole);
                userEntity.Status = AccountStatus.Approved;

                await dbContext.Administrators.AddAsync(user.ToAdministartorEntity(userEntity));

                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedCustomerAsync(DataContext dbContext)
        {
            await dbContext.Database.EnsureCreatedAsync();
            if (!await dbContext.Customers.AnyAsync() && await dbContext.Roles.AnyAsync())
            {
                var customerRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Customer);

                if (customerRole is null)
                {
                    return;
                }

                var user = new User
                {
                    FirstName = "Customer",
                    LastName = "1",
                    EmailAddress = "customer@abv.bg",
                    PhoneNumber = "+0123456789",
                    Password = "Customer1!",
                    RoleName = Roles.Customer,
                    Address = new Address
                    {
                        Country = "The United Kingdom",
                        City = "London",
                        Street = "Old Broad St.",
                        StreetNumber = "125",
                        BuildingNumber = "10",
                        ApartmentNumber = "5",
                        AdditionalInfo = "Central London"      
                    }
                };

                var userEntity = user.ToUserEntity(customerRole);
                userEntity.Status = AccountStatus.Approved;

                await dbContext.Customers.AddAsync(user.ToCustomerEntity(userEntity));

                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedLibrarianAsync(DataContext dbContext)
        {
            await dbContext.Database.EnsureCreatedAsync();
            if (!await dbContext.Librarians.AnyAsync() && await dbContext.Roles.AnyAsync())
            {
                var librarianRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Librarian);

                if (librarianRole is null)
                {
                    return;
                }

                var user = new User
                {
                    FirstName = "Librarian",
                    LastName = "1",
                    EmailAddress = "librarian@abv.bg",
                    PhoneNumber = "+0123456789",
                    Password = "Librarian1!",
                    RoleName = Roles.Librarian,
                };

                var userEntity = user.ToUserEntity(librarianRole);
                userEntity.Status = AccountStatus.Approved;

                await dbContext.Librarians.AddAsync(user.ToLibrarianEntity(userEntity));

                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedUsersAsync(DataContext dbContext)
        {
            await SeedAdministratorAsync(dbContext);
            await SeedCustomerAsync(dbContext);
            await SeedLibrarianAsync(dbContext);
        }
    }
}