using BusinessLayer.Enumerations;
using BusinessLayer.Models;

namespace BusinessLayer.Interfaces.BookItems;

public interface IBookItemsService
{
    Task<BookItem?> Get(Guid bookItemId);
    Task<BookItem> Create(BookItem bookItem);
    Task<BookItem?> Update(Guid bookItemId, BookItem bookItem);
    Task Delete(Guid bookItemId);
    Task<bool> Contains(Guid bookItemId);
    Task<BookItem?> UpdateBookItemStatus(Guid bookItemId, BookItemStatusEnumeration bookStatus);
    Task<bool> IsBarcodeExisting(string barcode);
}