using BusinessLayer.Models;
using System.Security.Claims;

namespace BusinessLayer.Interfaces.Users
{
    public interface IUsersService
    {
        Task<AuthenticatedUser> Authenticate(string email, string password, bool hashedPassword = false);
        Task Register(User user);
        Task<int> GetCount();
        Task<AuthenticatedUser> RefreshToken(ClaimsPrincipal user);
        Task<User> ApproveUser(Guid userId, ClaimsPrincipal approver);
        Task RejectUser(Guid userId, ClaimsPrincipal rejecter);
        Task<IEnumerable<User>> GetPendingCustomers();
        Task<IEnumerable<User>> GetPendingUsers();
        Task<User> GetUserById(Guid userId);
        Task<User> GetLibrarianById(Guid librarianId);
        Task<IEnumerable<User>> GetUsers(string filter);
    }
}