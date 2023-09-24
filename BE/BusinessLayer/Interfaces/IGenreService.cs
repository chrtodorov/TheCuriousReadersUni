using BusinessLayer.Models;

namespace BusinessLayer.Interfaces;

public interface IGenreService
{
    Task<int> GetCountAsync();
    Task<List<Genre>> GetGenresAsync();
}