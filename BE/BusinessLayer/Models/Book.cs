using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models;

public class Book
{
    public Guid BookId { get; set; }
    
    public string Isbn { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string CoverUrl { get; set; } = string.Empty;

    public Guid? PublisherId { get; set; }
    public Guid? BlobId { get; set; }

    public ICollection<Guid>? AuthorsIds { get; set; }

    public ICollection<BookItem>? BookItems { get; set; }
    public DateTime CreatedAt { get; set; }
}