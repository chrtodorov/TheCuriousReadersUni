using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models.Requests
{
    public class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}