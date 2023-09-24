using BusinessLayer.Models;

namespace BusinessLayer.Interfaces.BookRequests
{
    public interface IBookRequestsService
    {
        PagedList<BookRequestModel> GetAllRequests(PagingParameters bookRequestParameters);
        Task<BookRequestModel> MakeRequest(BookRequestModel bookRequest);
        Task<PagedList<BookRequestModel>> GetUserRequests(Guid customerId, PagingParameters bookRequestParameters);
        Task RejectRequest(Guid bookRequestId);
    }
}