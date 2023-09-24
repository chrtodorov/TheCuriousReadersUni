namespace BusinessLayer.Requests
{
    public class CommentRequest
    {
        public string Content { get; set; } = null!;
        public Guid BookId { get; set; }
    }
}