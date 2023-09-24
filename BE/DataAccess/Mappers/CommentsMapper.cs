using BusinessLayer.Enumerations;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccess.Entities;

namespace DataAccess.Mappers
{
    public static class CommentsMapper
    {
        public static Comment ToComment(this CommentEntity commentEntity)
        {
            return new Comment
            {
                CommentId =  commentEntity.CommentId,
                Content = commentEntity.Content,
                Status = commentEntity.Status,
                Book = commentEntity.Book.ToBookWithoutItems(),
                User = commentEntity.User.ToCustomerUserResponse(),
            };
        }

        public static Comment ToComment(this CommentRequest commentRequest, Guid userId)
        {
            return new Comment
            {
                Content = commentRequest.Content,
                BookId = commentRequest.BookId,
                UserId = userId,
                Status = CommentStatus.Pending
            };
        }

        public static CommentEntity ToCommentEntity(this Comment comment)
        {
            return new CommentEntity
            {
                Content = comment.Content,
                Status = comment.Status,
                BookId = comment.BookId,
                UserId = comment.UserId,
            };
        }

        public static CommentResponse ToCommentResponse(this Comment comment)
        {
            return new CommentResponse
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                Status = comment.Status,
                User = comment.User,
                Book = comment.Book
            };
        }
    }
}