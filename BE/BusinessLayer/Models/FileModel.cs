using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Models;

public class FileModel
{
    [Required]
    public IFormFile? ImageFile { get; set; }
}