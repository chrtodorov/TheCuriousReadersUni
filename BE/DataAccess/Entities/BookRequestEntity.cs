using BusinessLayer.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class BookRequestEntity : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BookRequestId { get; set; }

        public BookRequestStatus Status { get; set; } = BookRequestStatus.Pending;

        [Required]
        public CustomerEntity Customer { get; set; } = null!;
        public Guid RequestedBy { get; set; }

        public LibrarianEntity? Librarian { get; set; } = null!;
        public Guid? AuditedBy { get; set; }

        [Required]
        public BookItemEntity BookItem { get; set; } = null!;
        public Guid BookItemId { get; set; }

    }
}