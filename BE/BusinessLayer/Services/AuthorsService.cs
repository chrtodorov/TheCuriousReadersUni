using BusinessLayer.Helpers;
using BusinessLayer.Interfaces.Authors;
using BusinessLayer.Models;

namespace BusinessLayer.Services;

public class AuthorsService : IAuthorsService
{
    private readonly IAuthorsRepository _authorsRepository;

    public AuthorsService(IAuthorsRepository authorsRepository)
    {
        _authorsRepository = authorsRepository;
    }

    public async Task<Author?> Get(Guid authorId)
    {
        var author = await _authorsRepository.Get(authorId);
        if (author is null)
            throw new KeyNotFoundException("Author cannot be found!");
        return author;
    }

    public async Task<Author> Create(Author author)
    {
        if (await _authorsRepository.IsAuthorNameExisting(author.Name))
            throw new AppException($"Author with this name: {author.Name} is already existing!");

        return await _authorsRepository.Create(author);
    }

    public async Task<Author?> Update(Guid authorId, Author author)
    {
        if (!await _authorsRepository.Contains(authorId)) throw new KeyNotFoundException("Author cannot be found!");

        var authorFromDb = await _authorsRepository.Get(authorId);

        if (await _authorsRepository.IsAuthorNameExisting(author.Name) && authorFromDb!.Name != author.Name)
            throw new AppException($"Author with this name: {author.Name} is already existing!");

        return await _authorsRepository.Update(authorId, author);
    }

    public async Task Delete(Guid authorId)
    {
        if (!await _authorsRepository.Contains(authorId)) throw new KeyNotFoundException("Author cannot be found!");
        await _authorsRepository.Delete(authorId);
    }

    public async Task<bool> Contains(Guid id)
    {
        return await _authorsRepository.Contains(id);
    }

    public async Task<bool> IsAuthorNameExisting(string name)
    {
        return await _authorsRepository.IsAuthorNameExisting(name);
    }

    public async Task<PagedList<Author>> GetAuthors(AuthorParameters authorParameters)
    {
        return await _authorsRepository.GetAuthors(authorParameters);
    }
}