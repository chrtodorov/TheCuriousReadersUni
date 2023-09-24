using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using BusinessLayer.Interfaces.BookItems;
using BusinessLayer.Interfaces.Books;
using BusinessLayer.Models;
using BusinessLayer.Responses;

namespace BusinessLayer.Services;

public class BooksService : IBooksService
{
    private readonly IBlobService _blobService;
    private readonly IBookItemsRepository _bookItemsRepository;
    private readonly IBooksRepository _bookRepository;

    public BooksService(IBooksRepository bookRepository, IBookItemsRepository bookItemsRepository,
        IBlobService blobService)
    {
        _bookRepository = bookRepository;
        _bookItemsRepository = bookItemsRepository;
        _blobService = blobService;
    }

    public async Task<BookDetailsResponse?> Get(Guid bookId)
    {
        var book = await _bookRepository.Get(bookId);
        if (book is null)
            throw new KeyNotFoundException("Book not found!");

        return book;
    }

    public async Task<PagedList<Book>> GetBooks(BookParameters booksParameters)
    {
        return await _bookRepository.GetBooks(booksParameters);
    }

    public async Task<List<Book>> GetLatest()
    {
        return await _bookRepository.GetLatest();
    }

    public async Task<int> GetNumber()
    {
        return await _bookRepository.GetNumber();
    }

    public async Task<Book> Create(Book book)
    {
        if (await _bookRepository.IsIsbnExisting(book.Isbn))
            throw new AppException($"Book with this ISBN: {book.Isbn} already exists!");

        foreach (var bookItem in book.BookItems!)
            if (await _bookItemsRepository.IsBarcodeExisting(bookItem.Barcode))
                throw new AppException($"Book Copy with Barcode: {bookItem.Barcode} already exists!");

        return await _bookRepository.Create(book);
    }

    public async Task<Book?> Update(Guid bookId, Book book)
    {
        if (!await _bookRepository.Contains(bookId)) throw new KeyNotFoundException("Book cannot be found!");

        var bookFromDb = await _bookRepository.Get(bookId);

        if (await _bookRepository.IsIsbnExisting(book.Isbn) && bookFromDb!.Isbn != book.Isbn)
            throw new AppException($"Cannot update because this ISBN: {book.Isbn} already exists!");
        return await _bookRepository.Update(bookId, book);
    }

    public async Task Delete(Guid bookId)
    {
        var book = await Get(bookId);
        if (book is null)
            throw new KeyNotFoundException("Book cannot be found!");
        if (await _bookRepository.HasLoanedItems(bookId))
            throw new AppException($"There are active book loans for this book. Complete them before try to delete it!");
        var blobName = book.CoverUrl.Split('/').Last();
        await _bookRepository.Delete(bookId);
        await _blobService.DeleteAsync(blobName);
    }

    public async Task MakeUnavailable(Guid bookId)
    {
        var book = await Get(bookId);
        if (book is null)
            throw new KeyNotFoundException("Book cannot be found!");
        await _bookRepository.MakeUnavailable(bookId);
    }

    public async Task<bool> Contains(Guid bookId) => await _bookRepository.Contains(bookId);
    public async Task<bool> IsIsbnExisting(string isbn) => await _bookRepository.IsIsbnExisting(isbn);

    public PagedList<Book> GetReadBooks(Guid userId, PagingParameters pagingParameters)
        => _bookRepository.GetReadBooks(userId, pagingParameters);
}