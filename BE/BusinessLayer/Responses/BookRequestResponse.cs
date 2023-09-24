using BusinessLayer.Enumerations;
using BusinessLayer.Models;

namespace BusinessLayer.Responses
{
    public class BookRequestResponse
    {
        public DateTime CreatedAt { get; set; }

        public BookRequestStatus Status { get; set; }

        public Book Book { get; set; } = null!;
    }
}