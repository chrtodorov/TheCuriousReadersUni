using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models;

public class Publisher
{
    public Guid PublisherId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}