using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Entities;

namespace DataAccess.Mappers;

public static class AuthorMapper
{
    public static Author ToAuthor(this AuthorEntity authorEntity)
    {
        return new Author
        {
            AuthorId = authorEntity.AuthorId,
            Name = authorEntity.Name,
            Bio = authorEntity.Bio
        };
    }

    public static AuthorEntity ToAuthorEntity(this Author author)
    {
        return new AuthorEntity
        {
            Name = author.Name,
            Bio = author.Bio
        };
    }

    public static Author ToAuthor(this AuthorsRequest authorsCreateRequest)
    {
        return new Author
        {
            Name = authorsCreateRequest.Name.Trim(),
            Bio = authorsCreateRequest.Bio
        };
    }
}