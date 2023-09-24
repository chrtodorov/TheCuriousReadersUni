using BusinessLayer.Enumerations;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.BookLoans;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Entities;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BookLoansRepository : IBookLoansRepository
{
    private readonly DataContext _dbContext;

    public BookLoansRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

        public async Task CompleteLoan(Guid bookLoanId)
        {
            var bookLoanEntity = await GetBookLoansQuery()
                .FirstOrDefaultAsync(l => l.BookLoanId == bookLoanId);

        if (bookLoanEntity is null) throw new KeyNotFoundException("Book loan does not exist");

        bookLoanEntity.Status = BookLoanStatus.Completed;
        bookLoanEntity.BookItem.BookStatus = BookItemStatusEnumeration.Available;
        _dbContext.Update(bookLoanEntity);
        await _dbContext.SaveChangesAsync();
    }

    public PagedList<BookLoan> GetAll(PagingParameters pagingParameters)
    {
        var booksQuery = GetBookLoansQuery().Select(l => l.ToBookLoan());
        return PagedList<BookLoan>.ToPagedList(booksQuery, pagingParameters.PageNumber, pagingParameters.PageSize);
    }

    public PagedList<BookLoan> GetExpiring(PagingParameters pagingParameters)
    {
        var afterTwoWeeks = DateTime.UtcNow.AddDays(14);
        var booksQuery = GetBookLoansQuery()
            .Where(l => l.To <= afterTwoWeeks)
            .Select(l => l.ToBookLoan());
        return PagedList<BookLoan>.ToPagedList(booksQuery, pagingParameters.PageNumber, pagingParameters.PageSize);
    }

    public async Task<PagedList<BookLoan>> GetLoansById(Guid userId, PagingParameters pagingParameters)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists) throw new KeyNotFoundException("User does not exist");

        var loansQuery = GetBookLoansQuery()
            .Where(l => l.Customer.User.UserId == userId)
            .Select(l => l.ToBookLoan());
        return PagedList<BookLoan>.ToPagedList(loansQuery, pagingParameters.PageNumber, pagingParameters.PageSize);
    }

    public async Task<BookLoan> LoanBook(BookLoan bookLoan)
    {
        var customerExists = await _dbContext.Customers.AnyAsync(c => c.CustomerId == bookLoan.LoanedToId);
        if (!customerExists) throw new KeyNotFoundException("Customer does not exist");

        var bookItem = await _dbContext.BookItems.FirstOrDefaultAsync(i => i.BookItemId == bookLoan.BookItemId);
        if (bookItem is null)
            throw new KeyNotFoundException($"Book copy does not exist");
        if (bookItem.BookStatus == BookItemStatusEnumeration.Borrowed)
            throw new AppException("Book copy has already been borrowed");
        if (bookItem.BookStatus == BookItemStatusEnumeration.Available)
            throw new AppException("A book copy must be reserved and approved by librarian");
        if (bookItem.BookStatus == BookItemStatusEnumeration.NotAvailable)
            throw new AppException("Book copy is unavailable");

        var createdEntity = await _dbContext.BookLoans.AddAsync(bookLoan.ToBookLoanEntity());
        bookItem.BookStatus = BookItemStatusEnumeration.Borrowed;
        createdEntity.Entity.BookItem = bookItem;
        var bookRequest = await _dbContext.BookRequests.FirstAsync(i => i.BookItemId == bookLoan.BookItemId);
        bookRequest.Status = BookRequestStatus.Approved;
        await _dbContext.SaveChangesAsync();

        var fullCreatedEntity =
            await GetBookLoansQuery().FirstAsync(l => l.BookLoanId == createdEntity.Entity.BookLoanId);
        return fullCreatedEntity.ToBookLoan();
    }

    public async Task<BookLoan> ProlongLoan(Guid bookLoanId, ProlongRequest prolongRequest)
    {
        var bookLoanEntity = await GetBookLoansQuery()
            .FirstOrDefaultAsync(l => l.BookLoanId == bookLoanId);

        if (bookLoanEntity is null) throw new KeyNotFoundException($"Book loan does not exist");

        if (bookLoanEntity.To >= prolongRequest.ExtendedTo)
            throw new AppException("Requested end time is before the actual loan end time");

        bookLoanEntity.To = prolongRequest.ExtendedTo;
        bookLoanEntity.TimesExtended++;
        _dbContext.Update(bookLoanEntity);
        await _dbContext.SaveChangesAsync();

        return bookLoanEntity.ToBookLoan();
    }


    private IQueryable<BookLoanEntity> GetBookLoansQuery()
    {
        return _dbContext.BookLoans
            .Where(l => l.Status == BookLoanStatus.Active)
            .Include(l => l.Customer)
            .ThenInclude(c => c.User)
            .Include(l => l.BookItem)
            .ThenInclude(i => i.Book)
            .AsNoTracking();
    }
}