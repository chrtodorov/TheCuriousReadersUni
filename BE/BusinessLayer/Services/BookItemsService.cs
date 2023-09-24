using BusinessLayer.Enumerations;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.BookItems;
using BusinessLayer.Models;

namespace BusinessLayer.Services;

public class BookItemsService : IBookItemsService
{
    private readonly IBookItemsRepository _bookItemsRepository;

    public BookItemsService(IBookItemsRepository bookItemsRepository)
    {
        _bookItemsRepository = bookItemsRepository;
    }

    public async Task<BookItem> Create(BookItem bookItem)
    {
        if (await _bookItemsRepository.IsBarcodeExisting(bookItem.Barcode))
            throw new AppException($"Book Copy with this barcode: {bookItem.Barcode} already exists.");
        return await _bookItemsRepository.Create(bookItem);
    }

    public async Task Delete(Guid bookItemId)
    {
        if (!await _bookItemsRepository.Contains(bookItemId))
            throw new KeyNotFoundException("Book copy cannot be found!");
        await _bookItemsRepository.Delete(bookItemId);
    }


    public async Task<BookItem?> Get(Guid bookItemId)
    {
        var bookItem = await _bookItemsRepository.Get(bookItemId);
        if (bookItem is null) throw new KeyNotFoundException("Book copy cannot be found!");
        return bookItem;
    }

    public async Task<BookItem?> Update(Guid bookItemId, BookItem bookItem)
    {
        if (!await _bookItemsRepository.Contains(bookItemId))
            throw new KeyNotFoundException("Book copy cannot be found!");
        if (await _bookItemsRepository.IsBarcodeExisting(bookItem.Barcode))
            throw new AppException($"Book Copy with this barcode: {bookItem.Barcode} already exists.");
        return await _bookItemsRepository.Update(bookItemId, bookItem);
    }

    public async Task<bool> Contains(Guid bookItemId)
    {
        return await _bookItemsRepository.Contains(bookItemId);
    }

    public async Task<BookItem?> UpdateBookItemStatus(Guid bookItemId, BookItemStatusEnumeration bookStatus)
    {
        if (!await _bookItemsRepository.Contains(bookItemId))
            throw new KeyNotFoundException("Book copy cannot be found!");
        return await _bookItemsRepository.UpdateBookItemStatus(bookItemId, bookStatus);
    }

    public async Task<bool> IsBarcodeExisting(string barcode)
    {
        return await _bookItemsRepository.IsBarcodeExisting(barcode);
    }
}