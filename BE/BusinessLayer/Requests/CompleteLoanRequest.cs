namespace BusinessLayer.Requests
{
    public class CompleteLoanRequest
    {
        public Guid BookId { get; set; }
        public Guid LoanedToId { get; set; }
    }
}