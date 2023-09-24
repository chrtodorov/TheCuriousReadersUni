using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.BookLoans;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookLoansController : ControllerBase
{
    private readonly IBookLoansService bookLoansService;

    public BookLoansController(IBookLoansService bookLoansService)
    {
        this.bookLoansService = bookLoansService;
    }

    [HttpGet("User/{userId}")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> GetLoansByUserId(Guid userId, [FromQuery] PagingParameters pagingParameters)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var pagedList = await bookLoansService.GetLoansById(userId, pagingParameters);
        var responseData = pagedList.Data.Select(l => l.ToBookLoanResponse());
        return Ok(responseData.ToPagedList(pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize));
    }

    [HttpGet("Mine")]
    [Authorize(Policy = Policies.RequireCustomerRole)]
    public async Task<IActionResult> GetCurrentUserLoans([FromQuery] PagingParameters pagingParameters)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value!);
        var pagedList = await bookLoansService.GetLoansById(userId, pagingParameters);
        var responseData = pagedList.Data.Select(l => l.ToBookLoanResponse());
        return Ok(responseData.ToPagedList(pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize));
    }

    [HttpPost]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> Create(BookLoanRequest bookLoanRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userSpecificId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "RoleSpecificId")?.Value!);
        var createdLoan = await bookLoansService.LoanBook(bookLoanRequest.ToBookLoan(userSpecificId));
        return Ok(createdLoan.ToBookLoanResponse());
    }

    [HttpGet]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public IActionResult GetAll([FromQuery] PagingParameters pagingParameters)
    {
        var pagedList = bookLoansService.GetAll(pagingParameters);
        var responseData = pagedList.Data.Select(l => l.ToBookLoanResponse());
        return Ok(responseData.ToPagedList(pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize));
    }

    [HttpGet("expiring")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public IActionResult GetExpiring([FromQuery] PagingParameters pagingParameters)
    {
        var pagedList = bookLoansService.GetExpiring(pagingParameters);
        var responseData = pagedList.Data.Select(l => l.ToBookLoanResponse());
        return Ok(responseData.ToPagedList(pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize));
    }

    [HttpPut("{bookLoanId}")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> ProlongLoan(Guid bookLoanId, [FromBody] ProlongRequest prolongRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var bookLoan = await bookLoansService.ProlongLoan(bookLoanId, prolongRequest);
        return Ok(bookLoan.ToBookLoanResponse());
    }

    [HttpPut("Complete/{bookLoanId}")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> CompleteLoan(Guid bookLoanId, [FromBody] CompleteLoanRequest completeLoanRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            await bookLoansService.CompleteLoan(bookLoanId, completeLoanRequest);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("BookLoan", ex.Message);
            return BadRequest(ModelState);
        }
    }
}
