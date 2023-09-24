namespace BusinessLayer.Interfaces.UserBooks
{
    public interface IUserBooksRepository
    {
        Task Add(Guid userId, Guid bookId);
    }
}