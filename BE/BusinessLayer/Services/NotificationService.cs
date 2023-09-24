using BusinessLayer.Interfaces.BookLoans;
using BusinessLayer.Interfaces.Notifications;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class NotificationService : INotificationsService
    {
        private readonly INotificationsRepository notificationsRepository;
        public NotificationService(INotificationsRepository notificationsRepository)
        {
            this.notificationsRepository = notificationsRepository;
        }

        public PagedList<BookLoan> GetExpiredBookLoans(PagingParameters pagingParameters)
            => notificationsRepository.GetExpiredBookLoans(pagingParameters);

        public PagedList<BookLoan> GetExpiringBookLoans(PagingParameters pagingParameters)
            => notificationsRepository.GetExpiringBookLoans(pagingParameters);
    }
}