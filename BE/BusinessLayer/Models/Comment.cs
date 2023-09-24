using BusinessLayer.Enumerations;
using BusinessLayer.Responses;

namespace BusinessLayer.Models
{
    public class Comment
    {
        public Guid? CommentId { get; set; }
        public string Content { get; set; } = null!;
        public CommentStatus Status { get; set; }

        public Book Book { get; set; } = null!;
        public Guid BookId { get; set; }

        public UserResponse User { get; set; } = null!;
        public Guid UserId { get; set; }
    }

    
}