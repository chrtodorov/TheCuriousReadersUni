using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class AddressEntity:AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AddressId { get; set; }

        [Required]
        [MaxLength(60)]
        [RegularExpression(@"^([\sA-Za-zа-яА-Я])+$")]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        [RegularExpression(@"^([\sA-Za-zа-яА-Я])+$")]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string Street { get; set; } = null!;

        [Required]
        public string StreetNumber { get; set; } = null!;

        [MaxLength(65)]
        public string? BuildingNumber { get; set; }

        [MaxLength(65)]
        public string? ApartmentNumber { get; set; }

        [MaxLength(1028)]
        public string? AdditionalInfo { get; set; } = null!;

        public CustomerEntity Customer { get; set; } = null!;
    }
}