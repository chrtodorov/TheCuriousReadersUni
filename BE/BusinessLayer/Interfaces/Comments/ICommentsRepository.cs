using BusinessLayer.Models;

namespace BusinessLayer.Interfaces.Comments
{
    public interface ICommentsRepository 
    {
        PagedList<Comment> GetCommentsByBookId(Guid bookId, PagingParameters pagingParameters);
        PagedList<Comment> GetCommentsByUserId(Guid userId, PagingParameters pagingParameters);
        PagedList<Comment> GetPendingComments(PagingParameters pagingParameters);
        Task<Comment> AddComment(Comment comment);
        Task<Comment> ApproveComment(Guid commentId);
        Task<Comment> RejectComment(Guid commentId);
    }
}