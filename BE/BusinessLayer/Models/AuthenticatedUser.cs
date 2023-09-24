using System.Text.Json.Serialization;

namespace BusinessLayer.Models
{
    public record AuthenticatedUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        public AuthenticatedUser(string name, string email, string role, string jwtToken, string refreshToken)
        {
            this.Name = name;
            this.Email = email;
            this.Role = role;
            this.JwtToken = jwtToken;
            this.RefreshToken = refreshToken;
        }
    }
}