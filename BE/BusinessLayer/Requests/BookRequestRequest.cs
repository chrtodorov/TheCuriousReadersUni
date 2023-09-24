using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests
{
    public class BookRequestRequest
    {
        [Required]
        public Guid BookId { get; set; }
    }
}