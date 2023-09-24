using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests
{
    public class PublisherRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}