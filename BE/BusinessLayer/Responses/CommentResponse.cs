using BusinessLayer.Enumerations;
using BusinessLayer.Models;

namespace BusinessLayer.Responses
{
    public class CommentResponse
    {
        public Guid? CommentId { get; set; }
        public string Content { get; set; } = null!;
        public CommentStatus Status { get; set; }

        public Book Book { get; set; } = null!;

        public UserResponse User { get; set; } = null!;
    }
}