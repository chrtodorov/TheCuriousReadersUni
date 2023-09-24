using BusinessLayer.Interfaces;
using BusinessLayer.Models;

namespace BusinessLayer.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;

    public GenreService(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<int> GetCountAsync() => await _genreRepository.GetCountAsync();

    public async Task<List<Genre>> GetGenresAsync() => await _genreRepository.GetGenresAsync();
}