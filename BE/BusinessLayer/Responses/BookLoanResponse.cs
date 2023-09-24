using BusinessLayer.Models;

namespace BusinessLayer.Responses
{
    public class BookLoanResponse
    {
        public Guid BookLoanId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TimesExtended { get; set; }
        public Guid BookItemId { get; set; }
        public string BookItemBarcode { get; set; } = null!;
        public UserResponse LoanedTo { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}