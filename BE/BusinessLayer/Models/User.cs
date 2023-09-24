using BusinessLayer.Attributes;
using BusinessLayer.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public record User
    {

        [Required]
        public Guid UserId  { get; set; }

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; } = null!;

        [Email]
        [Required]
        public string EmailAddress { get; set; } = null!;

        [Required]
        [RegularExpression(@"\+[0-9]+")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(65, MinimumLength = 10)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{10,65}$")]
        public string Password { get; set; } = null!;

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Pending;

        [Required]
        public string RoleName { get; set; } = null!;

        public Address? Address { get; set; }
    }
}