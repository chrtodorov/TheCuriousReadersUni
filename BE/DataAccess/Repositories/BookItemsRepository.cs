using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.BookItems;
using BusinessLayer.Models;
using DataAccess.Entities;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories;

public class BookItemsRepository : IBookItemsRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<BookItemsRepository> _logger;

    public BookItemsRepository(DataContext dataContext, ILogger<BookItemsRepository> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<BookItem> Create(BookItem bookItem)
    {
        var bookItemEntity = bookItem.ToBookItemEntity();
        await _dataContext.BookItems.AddAsync(bookItemEntity);
        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }

        _logger.LogInformation("Create book item with {@BookItemId}", bookItemEntity.BookItemId);
        return bookItemEntity.ToBookItem();
    }

    public async Task Delete(Guid bookItemId)
    {
        var bookItemEntity = await GetById(bookItemId);

        if (bookItemEntity is not null)
        {
            _dataContext.BookItems.Remove(bookItemEntity);
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                throw;
            }

            _logger.LogInformation("Deleting Book Item with {@BookItemId}", bookItemId);
        }

        _logger.LogInformation("There is no such Book Item with {@BookItemId}", bookItemId);
    }

    public async Task<BookItem?> Get(Guid bookItemId)
    {
        _logger.LogInformation("Get Book Item with {@BookItemId}", bookItemId);
        var bookItemEntity = await GetById(bookItemId, false);
        return bookItemEntity?.ToBookItem();
    }

    public async Task<BookItem?> Update(Guid bookItemId, BookItem bookItem)
    {
        var bookItemEntity = await GetById(bookItemId);

        if (bookItemEntity is null) return null;

        bookItemEntity.Barcode = bookItem.Barcode;
        bookItemEntity.BorrowedDate = bookItem.BorrowedDate;
        bookItemEntity.ReturnDate = bookItem.ReturnDate;
        bookItemEntity.BookStatus = bookItem.BookStatus;

        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }


        _logger.LogInformation("Update Book Item with {@BookItemId}", bookItemEntity.BookItemId);
        return bookItemEntity.ToBookItem();
    }

    public async Task<bool> Contains(Guid bookItemId)
    {
        return await _dataContext.BookItems.AnyAsync(b => b.BookItemId == bookItemId);
    }

    public async Task<BookItem?> UpdateBookItemStatus(Guid bookItemId, BookItemStatusEnumeration bookStatus)
    {
        var bookItemStatusEntity = await GetById(bookItemId);
        if (bookItemStatusEntity is null)
            throw new KeyNotFoundException("Book Copy not found.");
        bookItemStatusEntity.BookStatus = bookStatus;

        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }

        _logger.LogInformation("Update Book Item Status with {@BookStatus}", bookItemStatusEntity.BookStatus);
        return bookItemStatusEntity.ToBookItem();
    }

    public async Task<bool> IsBarcodeExisting(string barcode)
    {
        return await _dataContext.BookItems.AnyAsync(bi => bi.Barcode == barcode);
    }

    public async Task<bool> HasAvailableItems(Guid bookId)
    {
        return await _dataContext.BookItems.AnyAsync(i => i.BookId == bookId);
    }

    public async Task<BookItemEntity?> GetById(Guid bookItemId, bool tracking = true)
    {
        var query = _dataContext.BookItems
            .Where(b => b.BookItemId == bookItemId);
        if (!tracking)
            query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}