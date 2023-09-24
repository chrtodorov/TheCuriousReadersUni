using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models;

public class Author
{
    public Guid AuthorId { get; set; }

    [MaxLength(30)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Bio { get; set; } = string.Empty;
}