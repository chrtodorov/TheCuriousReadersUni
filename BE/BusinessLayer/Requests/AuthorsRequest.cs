using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests;

public class AuthorsRequest
{
    [MaxLength(30)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Bio { get; set; } = string.Empty;
}