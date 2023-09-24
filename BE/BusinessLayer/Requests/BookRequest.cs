using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests;

public class BookRequest
{
    [Required]
    [MaxLength(17)]
    public string Isbn { get; set; } = string.Empty;


    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Genre { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string CoverUrl { get; set; } = string.Empty;

    [Required]
    public Guid? BlobId { get; set; }

    [Required]
    public Guid? PublisherId { get; set; }

    [Required(ErrorMessage = "At least one author is required!")]
    [MinLength(1)]
    public ICollection<Guid>? AuthorsIds { get; set; }

    public ICollection<BookItemsRequest>? BookCopies { get; set; }
}