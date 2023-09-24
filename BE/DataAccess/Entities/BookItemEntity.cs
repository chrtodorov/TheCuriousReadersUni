using BusinessLayer.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class BookItemEntity:AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid BookItemId { get; set; }

    [Required]
    [MaxLength(10)]
    public string Barcode { get; set; } = string.Empty;

    public DateTime BorrowedDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public BookItemStatusEnumeration BookStatus { get; set; }

    [Required]
    public BookEntity? Book { get; set; }
    public Guid? BookId { get; set; }

    public BookLoanEntity? BookLoan { get; set; }
    public Guid? BookLoanId { get; set; }

    public ICollection<BookRequestEntity> BookRequests { get; set; } = new HashSet<BookRequestEntity>();

}