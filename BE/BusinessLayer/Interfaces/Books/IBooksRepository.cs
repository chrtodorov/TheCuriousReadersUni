using BusinessLayer.Models;
using BusinessLayer.Responses;

namespace BusinessLayer.Interfaces.Books;

public interface IBooksRepository
{
    Task<BookDetailsResponse?> Get(Guid bookId);
    Task<PagedList<Book>> GetBooks(BookParameters booksParameters);
    PagedList<Book> GetReadBooks(Guid userId, PagingParameters pagingParameters);
    Task<List<Book>> GetLatest();
    Task<Book> Create(Book book);
    Task<Book?> Update(Guid bookId, Book book);
    Task Delete(Guid bookId);
    Task<bool> Contains(Guid bookId);
    Task<bool> IsIsbnExisting(string isbn);
    Task<int> GetNumber();
    Task MakeUnavailable(Guid bookId);
    Task<bool> HasLoanedItems(Guid bookId);
}