using BusinessLayer.Enumerations;
using BusinessLayer.Models;

namespace BusinessLayer.Responses
{
    public class LibrarianBookRequestResponse 
    {
        public Guid BookRequestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookRequestStatus Status { get; set; }
        public Guid RequestedById { get; set; }
        public Guid BookCopyId { get; set; }

        public Book Book { get; set; } = null!;
        public UserResponse RequestedBy { get; set; } = null!;
    }
}