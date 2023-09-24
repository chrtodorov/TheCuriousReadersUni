using BusinessLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IGenreRepository
    {
        Task<int> GetCountAsync();
        Task<List<Genre>> GetGenresAsync();
    }
}