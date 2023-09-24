using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.Authors;
using BusinessLayer.Models;
using DataAccess.Entities;
using DataAccess.Mappers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories;

public class AuthorsRepository : IAuthorsRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<AuthorsRepository> _logger;

    public AuthorsRepository(DataContext dataContext, ILogger<AuthorsRepository> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<Author?> Get(Guid authorId)
    {
        _logger.LogInformation("Get Author with {@AuthorId}", authorId);

        var authorEntity = await GetById(authorId, false);

        return authorEntity?.ToAuthor();
    }

    public Task<PagedList<Author>> GetAuthors(AuthorParameters authorParameters)
    {
        var query = _dataContext.Authors.AsNoTracking();

        if (!string.IsNullOrEmpty(authorParameters.Name))
            query = query.Where(a => a.Name.Contains(authorParameters.Name));

        _logger.LogInformation("Get all authors");

        return Task.FromResult(PagedList<Author>.ToPagedList(query
                .OrderBy(b => b.Name)
                .Select(b => b.ToAuthor()),
            authorParameters.PageNumber,
            authorParameters.PageSize));
    }

    public async Task<Author> Create(Author author)
    {
        var authorEntity = author.ToAuthorEntity();

        await _dataContext.Authors.AddAsync(authorEntity);

        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }

        _logger.LogInformation("Create Author with {@AuthorId}", authorEntity.AuthorId);
        return authorEntity.ToAuthor();
    }

    public async Task<Author?> Update(Guid authorId, Author author)
    {
        var authorEntity = await GetById(authorId);

        if (authorEntity is null)
            return null;

        authorEntity.Name = author.Name;
        authorEntity.Bio = author.Bio;

        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }

        _logger.LogInformation("Update Author with {@AuthorId}", authorEntity.AuthorId);

        return authorEntity.ToAuthor();
    }

    public async Task Delete(Guid authorId)
    {
        var authorEntity = await GetById(authorId);

        if (authorEntity is not null)
        {
            _dataContext.Authors.Remove(authorEntity);
            try
            {
                await _dataContext.SaveChangesAsync();
                _logger.LogInformation("Deleting Author with {@AuthorId}", authorId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                if (e.GetBaseException() is SqlException {Number: 547})
                    throw new AppException("Must delete all books from this author before deleting it.");
                throw;
            }
        }
    }

    public async Task<bool> Contains(Guid id)
    {
        return await _dataContext.Authors.AnyAsync(a => a.AuthorId == id);
    }

    public async Task<bool> IsAuthorNameExisting(string name)
    {
        return await _dataContext.Authors.AnyAsync(a => a.Name == name);
    }

    public async Task<AuthorEntity?> GetById(Guid authorId, bool tracking = true)
    {
        var query = _dataContext.Authors
            .Where(a => a.AuthorId == authorId);

        if (!tracking)
            query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}