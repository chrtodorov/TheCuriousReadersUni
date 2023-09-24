using BusinessLayer.Models;
using BusinessLayer.Requests;
using System;

namespace BusinessLayer.Interfaces.BookLoans
{
    public interface IBookLoansRepository
    {
        Task<BookLoan> LoanBook(BookLoan bookLoan);
        Task CompleteLoan(Guid bookLoanId);
        Task<PagedList<BookLoan>> GetLoansById(Guid userId, PagingParameters pagingParameters);
        Task<BookLoan> ProlongLoan(Guid bookLoanId, ProlongRequest prolongRequest);
        PagedList<BookLoan> GetAll(PagingParameters pagingParameters);
        PagedList<BookLoan> GetExpiring(PagingParameters pagingParameters);
        
    }
}