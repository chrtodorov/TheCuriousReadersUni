using BusinessLayer.Models;

namespace BusinessLayer.Interfaces.Notifications
{
    public interface INotificationsRepository
    {
        PagedList<BookLoan> GetExpiringBookLoans(PagingParameters pagingParameters);
        PagedList<BookLoan> GetExpiredBookLoans(PagingParameters pagingParameters);
    }
}