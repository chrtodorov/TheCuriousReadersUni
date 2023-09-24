using BusinessLayer.Enumerations;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.BookRequests;
using BusinessLayer.Models;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories;

public class BookRequestsRepository : IBookRequestsRepository
{
    private readonly DataContext _dbContext;
    private readonly ILogger<BookRequestsRepository> _logger;

    public BookRequestsRepository(DataContext dbContext, ILogger<BookRequestsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public PagedList<BookRequestModel> GetAllRequests(PagingParameters bookRequestParameters)
    {
        var bookRequestsQuery = _dbContext.BookRequests
            .Where(r => r.Status == BookRequestStatus.Pending)
            .Include(r => r.Customer)
            .ThenInclude(c => c.User)
            .Include(r => r.BookItem)
            .ThenInclude(i => i.Book)
            .ThenInclude(b => b.Authors)
            .Include(r => r.BookItem)
            .ThenInclude(i => i.Book)
            .ThenInclude(b => b.Publisher)
            .Select(r => r.ToBookRequest(true))
            .AsNoTracking();

        return PagedList<BookRequestModel>.ToPagedList(bookRequestsQuery, bookRequestParameters.PageNumber,
            bookRequestParameters.PageSize);
    }

    public async Task<PagedList<BookRequestModel>> GetUserRequests(Guid customerId,
        PagingParameters bookRequestParameters)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
        if (customer is null) throw new KeyNotFoundException("Customer does not exist");

        var bookRequestsQuery = _dbContext.BookRequests
            .Where(r => r.RequestedBy == customerId && r.Status == BookRequestStatus.Pending)
            .Include(r => r.BookItem)
            .ThenInclude(i => i.Book)
            .ThenInclude(b => b.Authors)
            .Include(r => r.BookItem)
            .ThenInclude(i => i.Book)
            .ThenInclude(b => b.Publisher)
            .Select(r => r.ToBookRequest(false));

        return PagedList<BookRequestModel>.ToPagedList(bookRequestsQuery, bookRequestParameters.PageNumber,
            bookRequestParameters.PageSize);
    }

    public async Task<BookRequestModel> MakeRequest(BookRequestModel bookRequest)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(i => i.BookId == bookRequest.BookId);
        if (book is null) throw new KeyNotFoundException("Book does not exist");

        var alreadyRequestedBook = await _dbContext.BookRequests
            .AnyAsync(r => r.RequestedBy == bookRequest.RequestedBy && r.BookItem.BookId == book.BookId);
        if (alreadyRequestedBook)
            throw new AppException("This book is already requested!");

        var bookItem = await _dbContext.BookItems
            .FirstOrDefaultAsync(i =>
                i.BookId == bookRequest.BookId && i.BookStatus == BookItemStatusEnumeration.Available);

        if (bookItem is null)
            throw new AppException("There are no available copies of this book!");
        var createdRequest =
            await _dbContext.BookRequests.AddAsync(bookRequest.ToBookRequestEntity(bookItem.BookItemId));
        bookItem.BookStatus = BookItemStatusEnumeration.Reserved;
        await _dbContext.SaveChangesAsync();
        return createdRequest.Entity.ToBookRequest();
    }

    public async Task RejectRequest(Guid bookRequestId)
    {
        var bookRequest = await _dbContext.BookRequests.FirstOrDefaultAsync(r => r.BookRequestId == bookRequestId);
        if (bookRequest is null)
            throw new KeyNotFoundException("Book request does not exist");

        bookRequest.Status = BookRequestStatus.Rejected;
        _dbContext.Update(bookRequest);
        await _dbContext.SaveChangesAsync();
    }
}