using BusinessLayer.Models;

namespace BusinessLayer.Responses
{
    public class BookDetailsResponse
    {
        public Guid BookId { get; set; }

        public string Isbn { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public string CoverUrl { get; set; } = string.Empty;

        public Guid?  BlobId  { get; set; }

        public Publisher? Publisher { get; set; }

        public List<Author>? Authors { get; set; }

        public List<BookItem>? BookCopies { get; set; }
    }
}