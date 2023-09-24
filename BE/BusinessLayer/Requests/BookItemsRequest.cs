using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests;

public class BookItemsRequest
{
    [Required]
    [MaxLength(10)]
    [StringLength(10)]
    public string Barcode { get; set; } = string.Empty;
}