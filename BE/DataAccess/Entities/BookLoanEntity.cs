using BusinessLayer.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class BookLoanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BookLoanId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TimesExtended { get; set; }
        public BookLoanStatus Status { get; set; }

        [Required]
        public CustomerEntity Customer { get; set; } = null!;
        public Guid LoanedTo { get; set; }

        public LibrarianEntity? Librarian { get; set; } = null!;
        public Guid? LoanedBy { get; set; }

        [Required]
        public BookItemEntity BookItem { get; set; } = null!;
    }
}