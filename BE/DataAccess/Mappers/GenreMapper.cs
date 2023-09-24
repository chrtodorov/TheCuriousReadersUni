using BusinessLayer.Models;
using DataAccess.Entities;

namespace DataAccess.Mappers;

public static class GenreMapper
{
    public static Genre ToGenre(this GenreEntity genreEntity)
    {
        return new Genre
        {
            Name = genreEntity.Name
        };
    }
}