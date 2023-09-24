using BusinessLayer.Responses;

namespace BusinessLayer.Models
{
    public class BookLoan
    {
        public Guid BookLoanId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TimesExtended { get; set; }
        public Guid BookItemId { get; set; }
        public string BookItemBarcode { get; set; } = null!;
        public UserResponse LoanedTo { get; set; } = null!;
        public Guid LoanedToId { get; set; }
        public UserResponse LoanedBy { get; set; } = null!;
        public Guid LoanedById { get; set; }
        public Book Book { get; set; } = null!;
    }
}