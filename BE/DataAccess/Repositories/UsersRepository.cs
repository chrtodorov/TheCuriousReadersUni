using System.Security.Claims;
using BusinessLayer.Enumerations;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.Users;
using BusinessLayer.Models;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext _dbContext;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(DataContext dbContext, ILogger<UsersRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<User> GetUser(string email, string password, bool hashedPassword = false)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.EmailAddress == email);

        if (user is null) throw new KeyNotFoundException($"User with email: {email} does not exist");

        if (user.Status == AccountStatus.Pending) throw new AppException($"User with email: {email} is not approved");

        var passwordIsValid =
            hashedPassword ? user.Password == password : BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!passwordIsValid) throw new AppException("Invalid password was provided");

        _logger.LogInformation("Get user: {@userEmail}", email);

        return user.ToUser();
    }

    public async Task<User> GetUser(string email)
    {
        var userEntity = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.EmailAddress == email);

        if (userEntity is null) throw new KeyNotFoundException($"User with email: {email} does not exist");

        return userEntity.ToUser();
    }

    public async Task<Guid> GetUserSpecificId(Guid userId, string roleName)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is null) throw new KeyNotFoundException($"User with id: {userId} does not exist");
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

        if (role is null) throw new KeyNotFoundException($"Role with name {roleName} does not exist!");

        switch (role.Name)
        {
            case Roles.Administrator:
                var admin = await _dbContext.Administrators
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.User.UserId == userId);
                if (admin is null)
                    throw new AppException($"There is a mismatch between user with id: {userId} and role: {roleName}");

                return admin.AdministartorId;

            case Roles.Librarian:
                var librarian = await _dbContext.Librarians
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.User.UserId == userId);
                if (librarian is null)
                    throw new AppException($"There is a mismatch between user with id: {userId} and role: {roleName}");

                return librarian.LibrarianId;

            case Roles.Customer:
                var customer = await _dbContext.Customers
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.User.UserId == userId);
                if (customer is null)
                    throw new AppException($"There is a mismatch between user with id: {userId} and role: {roleName}");

                return customer.CustomerId;

            default:
                throw new KeyNotFoundException($"Role: {roleName} does not exist");
        }
    }

    public async Task<int> GetCount()
    {
        var query = await _dbContext.Customers
            .CountAsync();
        return query;
    }

    public async Task Register(User user)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == user.RoleName.ToLower());

        if (role is null) throw new KeyNotFoundException($"Role with name {user.RoleName} does not exist!");

        if (user.RoleName.ToLower() == Roles.Customer && user.Address is null)
            throw new AppException("The address field is required for customers!");

        var emailExists = await _dbContext.Users.AnyAsync(u => u.EmailAddress == user.EmailAddress);

        if (emailExists) throw new AppException($"Email: {user.EmailAddress} already exists!");

        var userEntity = user.ToUserEntity(role);
        userEntity.Status = AccountStatus.Pending;

        await _dbContext.Users.AddAsync(userEntity);

        switch (role.Name)
        {
            case Roles.Administrator:
                await _dbContext.Administrators.AddAsync(user.ToAdministartorEntity(userEntity));
                break;

            case Roles.Librarian:
                userEntity.Status = AccountStatus.Approved;
                await _dbContext.Librarians.AddAsync(user.ToLibrarianEntity(userEntity));
                break;

            case Roles.Customer:
                await _dbContext.Customers.AddAsync(user.ToCustomerEntity(userEntity));
                break;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> ApproveUser(Guid userId, ClaimsPrincipal approver)
    {
        var userEntity = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (userEntity is null) throw new KeyNotFoundException($"User with id: {userId} does not exist");

        if (approver.IsInRole(Roles.Librarian) && userEntity.Role.Name != Roles.Customer)
            throw new InvalidOperationException();

        userEntity.Status = AccountStatus.Approved;
        _dbContext.Update(userEntity);
        await _dbContext.SaveChangesAsync();

        return userEntity.ToUser();
    }

    public async Task RejectUser(Guid userId, ClaimsPrincipal rejecter)
    {
        var userEntity = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (userEntity is null) throw new KeyNotFoundException($"User with id: {userId} does not exist");

        if (rejecter.IsInRole(Roles.Librarian) && userEntity.Role.Name != Roles.Customer)
            throw new InvalidOperationException();

        _dbContext.Remove(userEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetPendingUsers()
    {
        return await _dbContext.Users
            .Include(u => u.Role)
            .Where(u => u.Status == AccountStatus.Pending)
            .Select(u => u.ToUser())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetPendingCustomers()
    {
        return await _dbContext.Users
            .Include(u => u.Role)
            .Where(u => u.Status == AccountStatus.Pending && u.Role.Name == Roles.Customer)
            .Select(u => u.ToUser())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User> GetUserById(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Customers)
            .ThenInclude(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        var customerEntity = await _dbContext.Customers
            .Include(u => u.User)
            .FirstOrDefaultAsync(c => c.User.UserId == userId);

        return user.ToUserWithAddress(customerEntity.Address);
    }

    public async Task<User> GetLibrarianById(Guid librarianId)
    {
        var librarian = await _dbContext.Users
            .Include(u => u.Librarians)
            .FirstOrDefaultAsync(u => u.UserId == librarianId);

        return librarian.ToUserLibrarian();
    }


    public async Task<IEnumerable<User>> GetUsers(string filter)
    {
        return await _dbContext.Users
            .Where(u => u.EmailAddress.Contains(filter))
            .Include(u => u.Role)
            .Select(u => u.ToUser())
            .ToListAsync();
    }
}