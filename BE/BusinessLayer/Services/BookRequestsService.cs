using BusinessLayer.Interfaces.BookItems;
using BusinessLayer.Interfaces.BookRequests;
using BusinessLayer.Models;

namespace BusinessLayer.Services;

public class BookRequestsService : IBookRequestsService
{
    private readonly IBookItemsRepository bookItemsRepository;
    private readonly IBookRequestsRepository bookRequestsRepository;

    public BookRequestsService(IBookRequestsRepository bookRequestsRepository, IBookItemsRepository bookItemsRepository)
    {
        this.bookRequestsRepository = bookRequestsRepository;
        this.bookItemsRepository = bookItemsRepository;
    }

    public PagedList<BookRequestModel> GetAllRequests(PagingParameters bookRequestParameters)
    {
        return bookRequestsRepository.GetAllRequests(bookRequestParameters);
    }

    public async Task<PagedList<BookRequestModel>> GetUserRequests(Guid customerId,
        PagingParameters bookRequestParameters)
    {
        return await bookRequestsRepository.GetUserRequests(customerId, bookRequestParameters);
    }


    public async Task<BookRequestModel> MakeRequest(BookRequestModel bookRequest)
    {
        var hasAvailableBookItems = await bookItemsRepository.HasAvailableItems(bookRequest.BookId);
        if (!hasAvailableBookItems)
            throw new KeyNotFoundException($"There are no available copies of this book!");
        return await bookRequestsRepository.MakeRequest(bookRequest);
    }

    public async Task RejectRequest(Guid bookRequestId)
    {
        await bookRequestsRepository.RejectRequest(bookRequestId);
    }
}