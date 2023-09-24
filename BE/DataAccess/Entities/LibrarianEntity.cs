using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class LibrarianEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LibrarianId { get; set; }

        [Required]
        public UserEntity User { get; set; } = null!;

        public ICollection<BookLoanEntity> BookLoans { get; set; } = new HashSet<BookLoanEntity>();
        public ICollection<BookRequestEntity> BookRequests { get; set; } = new HashSet<BookRequestEntity>();
    }
}