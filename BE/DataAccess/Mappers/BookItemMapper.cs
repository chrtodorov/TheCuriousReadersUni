using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Entities;

namespace DataAccess.Mappers;

public static class BookItemMapper
{
    public static BookItemEntity ToBookItemEntity(this BookItem bookItem)
    {
        return new BookItemEntity
        {
            Barcode = bookItem.Barcode,
            BorrowedDate = bookItem.BorrowedDate,
            ReturnDate = bookItem.ReturnDate,
            BookStatus = bookItem.BookStatus
        };
    }

    public static BookItem ToBookItem(this BookItemEntity bookItemEntity)
    {
        return new BookItem
        {
            BookItemId = bookItemEntity.BookItemId,
            Barcode = bookItemEntity.Barcode,
            BorrowedDate = bookItemEntity.BorrowedDate,
            ReturnDate = bookItemEntity.ReturnDate,
            BookStatus = bookItemEntity.BookStatus
        };
    }

    public static BookItem ToBookItem(this BookItemsRequest bookItemsRequest)
    {
        return new BookItem
        {
            Barcode = bookItemsRequest.Barcode
        };
    }
}