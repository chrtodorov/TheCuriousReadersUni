namespace DataAccess.Entities
{
    public class BookAuthor
    {
        public Guid BookId { get; set; }
        public Guid AuthorId { get; set; }


        public virtual BookEntity? BookEntity { get; set; }
        public virtual AuthorEntity? AuthorEntity { get; set; }
    }
}