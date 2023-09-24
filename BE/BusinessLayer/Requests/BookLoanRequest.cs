using BusinessLayer.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Requests
{
    public class BookLoanRequest
    {
        [Required]
        [ValidDate]
        public DateTime From { get; set; }

        [Required]
        [ValidDate]
        public DateTime To { get; set; }

        [Required]
        public Guid LoanedCopy { get; set; }

        [Required]
        public Guid LoanedTo { get; set; }
    }
}