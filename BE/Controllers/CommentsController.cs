using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.Comments;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentsService commentsService;

    public CommentsController(ICommentsService commentsService)
    {
        this.commentsService = commentsService;
    }

    [HttpGet("[action]")]
    public IActionResult Pending([FromQuery] PagingParameters pagingParameters)
    {
        var comments = commentsService.GetPendingComments(pagingParameters);
        return Ok(comments);
    }

    [HttpGet("User/{userId}")]
    public IActionResult GetByUserId(Guid userId, [FromQuery] PagingParameters pagingParameters)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var comments = commentsService.GetCommentsByUserId(userId, pagingParameters);
        return Ok(comments);
    }

    [HttpGet("Book/{bookId}")]
    public IActionResult GetByBookId(Guid bookId, [FromQuery] PagingParameters pagingParameters)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var comments = commentsService.GetCommentsByBookId(bookId, pagingParameters);
        return Ok(comments);
    }

        [HttpPost]
        [Authorize(Policy = Policies.RequireCustomerRole)]
        public async Task<IActionResult> AddComment([FromBody] CommentRequest commentRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value!);
        var createdComment = await commentsService.AddComment(commentRequest.ToComment(userId));
        return Ok(createdComment.ToCommentResponse());
    }

    [HttpPut("[action]/{commentId}")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> Approve(Guid commentId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var approvedComment = await commentsService.ApproveComment(commentId);
        return Ok(approvedComment.ToCommentResponse());
    }

    [HttpPut("[action]/{commentId}")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> Reject(Guid commentId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var rejectedComment = await commentsService.RejectComment(commentId);
        return Ok(rejectedComment.ToCommentResponse());
    }
}