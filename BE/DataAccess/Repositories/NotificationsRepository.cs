using BusinessLayer.Interfaces.Notifications;
using BusinessLayer.Models;
using DataAccess.Entities;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly DataContext _dbContext;
        const int DAYS_UNTIL_RETURN = 14;

        public NotificationsRepository(DataContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public PagedList<BookLoan> GetExpiringBookLoans(PagingParameters pagingParameters)
        {
            var twoWeeksFromNow = DateTime.UtcNow.AddDays(DAYS_UNTIL_RETURN);
            var bookLoans = GetQueryOfBookLoans().Where(bookLoanEntity => bookLoanEntity.To <= twoWeeksFromNow)
                .Select(bookLoanEntity => bookLoanEntity.ToBookLoan());

            return PagedList<BookLoan>.ToPagedList(bookLoans, pagingParameters.PageNumber, pagingParameters.PageSize);
        }
        
        public PagedList<BookLoan> GetExpiredBookLoans(PagingParameters pagingParameters)
        {
            var twoWeeksFromNow = DateTime.Now.AddDays(DAYS_UNTIL_RETURN);
            var bookLoans = GetQueryOfBookLoans().Where(bookLoanEntity => bookLoanEntity.To > twoWeeksFromNow)
                .Select(bookLoanEntity => bookLoanEntity.ToBookLoan());

            return PagedList<BookLoan>.ToPagedList(bookLoans, pagingParameters.PageNumber, pagingParameters.PageSize);
        }

        private IQueryable<BookLoanEntity> GetQueryOfBookLoans()
        {
            return _dbContext.BookLoans
                .Include(bookLoan => bookLoan.Customer)
                .ThenInclude(customer => customer.User)
                .Include(bookLoanEntity => bookLoanEntity.BookItem)
                .ThenInclude(bookItemEntity => bookItemEntity.Book)
                .AsNoTracking();
        }
    }
}