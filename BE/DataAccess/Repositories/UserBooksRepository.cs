using BusinessLayer.Interfaces.UserBooks;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserBooksRepository : IUserBooksRepository
    {
        private readonly DataContext _dbContext;

        public UserBooksRepository(DataContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task Add(Guid userId, Guid bookId)
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                throw new ArgumentException("User does not exist");
            }

            var bookExists = await _dbContext.Books.AnyAsync(u => u.BookId == bookId);
            if (!bookExists)
            {
                throw new ArgumentException("Book does not exist");
            }

            await _dbContext.AddAsync(new UserBooks
            {
                UserId = userId,
                BookId = bookId,
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}