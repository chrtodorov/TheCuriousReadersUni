using BusinessLayer.Models;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccess.Entities;

namespace DataAccess.Mappers
{
    public static class BookLoansMapper
    {
        public static BookLoan ToBookLoan(this BookLoanEntity bookLoanEntity)
        {
            return new BookLoan
            {
                BookLoanId = bookLoanEntity.BookLoanId,
                From = bookLoanEntity.From,
                To = bookLoanEntity.To,
                TimesExtended = bookLoanEntity.TimesExtended,
                LoanedTo = bookLoanEntity.Customer.ToUserResponse(),
                BookItemId = bookLoanEntity.BookItem.BookItemId,
                BookItemBarcode = bookLoanEntity.BookItem.Barcode,
                Book = bookLoanEntity.BookItem.Book.ToBookWithoutItems()
            };
        }

        public static BookLoan ToBookLoan(this BookLoanRequest bookLoanRequest, Guid loanedById)
        {
            return new BookLoan
            {
                From = bookLoanRequest.From,
                To = bookLoanRequest.To,
                TimesExtended = 0,
                BookItemId = bookLoanRequest.LoanedCopy,
                LoanedToId = bookLoanRequest.LoanedTo,
                LoanedById = loanedById,
            };
        }


        public static BookLoanEntity ToBookLoanEntity(this BookLoan bookLoan)
        {
            return new BookLoanEntity
            {
                From = bookLoan.From,
                To = bookLoan.To,
                TimesExtended = bookLoan.TimesExtended,
                LoanedTo = bookLoan.LoanedToId,
                LoanedBy = bookLoan.LoanedById,
            };
        }

        public static BookLoanResponse ToBookLoanResponse(this BookLoan bookLoan)
        {
            return new BookLoanResponse
            {
                BookLoanId = bookLoan.BookLoanId,
                From = bookLoan.From,
                To = bookLoan.To,
                TimesExtended = bookLoan.TimesExtended,
                BookItemId = bookLoan.BookItemId,
                BookItemBarcode = bookLoan.BookItemBarcode,
                Book = bookLoan.Book,
                LoanedTo = bookLoan.LoanedTo,
            };
        }

        public static PagedList<BookLoanResponse> ToPagedList(this IEnumerable<BookLoanResponse> data, int totalCount, int currentPage, int pageSize)
        {
            return new PagedList<BookLoanResponse>(data.ToList(), totalCount, currentPage, pageSize);
        }
    }
}
