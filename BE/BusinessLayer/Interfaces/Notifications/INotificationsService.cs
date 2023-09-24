using BusinessLayer.Models;

namespace BusinessLayer.Interfaces.Notifications
{
    public interface INotificationsService
    {
        PagedList<BookLoan> GetExpiringBookLoans(PagingParameters pagingParameters);
        PagedList<BookLoan> GetExpiredBookLoans(PagingParameters pagingParameters);
    }
}