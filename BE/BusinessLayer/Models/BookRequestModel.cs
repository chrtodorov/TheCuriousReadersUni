using BusinessLayer.Enumerations;
using BusinessLayer.Responses;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class BookRequestModel
    {
        public Guid? BookRequestId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public BookRequestStatus Status { get; set; }

        [Required]
        public Guid RequestedBy { get; set; }

        public UserResponse Customer { get; set; } = null!;

        public Guid? AuditedBy { get; set; }

        [Required]
        public Book Book { get; set; } = null!;
        public Guid BookId { get; set; }

        public Guid BookItemId { get; set; }
    }
}