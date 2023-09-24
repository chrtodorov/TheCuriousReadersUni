using BusinessLayer.Interfaces.Comments;
using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly ICommentsRepository commentsRepository;

        public CommentsService(ICommentsRepository commentsRepository)
        {
            this.commentsRepository = commentsRepository;
        }

        public async Task<Comment> AddComment(Comment comment)
            => await commentsRepository.AddComment(comment);

        public async Task<Comment> ApproveComment(Guid commentId)
            => await commentsRepository.ApproveComment(commentId);

        public async Task<Comment> RejectComment(Guid commentId)
            => await commentsRepository.RejectComment(commentId);

        public PagedList<Comment> GetCommentsByBookId(Guid bookId, PagingParameters pagingParameters)
            => commentsRepository.GetCommentsByBookId(bookId, pagingParameters);

        public PagedList<Comment> GetCommentsByUserId(Guid userId, PagingParameters pagingParameters)
            => commentsRepository.GetCommentsByUserId(userId, pagingParameters);

        public PagedList<Comment> GetPendingComments(PagingParameters pagingParameters)
            => commentsRepository.GetPendingComments(pagingParameters);
    }
}