using BusinessLayer.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models;

public class BookItem
{
    public Guid BookItemId { get; set; }

    [Required]
    [MaxLength(10)]
    [StringLength(10)]
    public string Barcode { get; set; } = string.Empty;

    public DateTime BorrowedDate { get; set; }

    public DateTime ReturnDate { get; set; }

    [Required]
    public BookItemStatusEnumeration BookStatus { get; set; }
}