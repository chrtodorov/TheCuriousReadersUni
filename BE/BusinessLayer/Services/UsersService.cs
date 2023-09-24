using BusinessLayer.Interfaces.Users;
using BusinessLayer.Models;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace BusinessLayer
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IConfiguration configuration;

        public UsersService(IUsersRepository usersRepository, IConfiguration configuration)
        {
            this.usersRepository = usersRepository;
            this.configuration = configuration;
        }

        public async Task<AuthenticatedUser> Authenticate(string email, string password, bool hashedPassword = false)
        {
            var user = await usersRepository.GetUser(email, password, hashedPassword);
            var userSpecificId = await usersRepository.GetUserSpecificId(user.UserId, user.RoleName);

            var key = configuration.GetSection("JwtSecret").Value;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.UserId.ToString()),
                    new Claim("RoleSpecificId", userSpecificId.ToString()),
                    new Claim(ClaimTypes.Email, user.EmailAddress),
                    new Claim(ClaimTypes.Role, user.RoleName),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwtTokenString = tokenHandler.WriteToken(jwtToken);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresOn = DateTime.UtcNow.AddDays(7),
            };

            return new AuthenticatedUser($"{user.FirstName} {user.LastName}", user.EmailAddress, user.RoleName, jwtTokenString, refreshToken.Token);

        }

        public async Task Register(User user) =>
            await usersRepository.Register(user);

        public async Task<AuthenticatedUser> RefreshToken(ClaimsPrincipal user)
        {
            var userEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var userModel = await usersRepository.GetUser(userEmail!);

            return await Authenticate(userModel.EmailAddress, userModel.Password, true);
        }
            
        public async Task<User> ApproveUser(Guid userId, ClaimsPrincipal approver) =>
            await usersRepository.ApproveUser(userId, approver);

        public async Task RejectUser(Guid userId, ClaimsPrincipal rejecter) =>
            await usersRepository.RejectUser(userId, rejecter);

        public async Task<IEnumerable<User>> GetPendingUsers() =>
            await usersRepository.GetPendingUsers();

        public async Task<IEnumerable<User>> GetPendingCustomers() =>
            await usersRepository.GetPendingCustomers();
        public async Task<User> GetUserById(Guid userId) =>
            await usersRepository.GetUserById(userId);
        public async Task<User> GetLibrarianById(Guid librarianId) =>
            await usersRepository.GetLibrarianById(librarianId);

        public async Task<int> GetCount() =>
            await usersRepository.GetCount();

        public async Task<IEnumerable<User>> GetUsers(string filter) => 
            await usersRepository.GetUsers(filter);
    }
}
