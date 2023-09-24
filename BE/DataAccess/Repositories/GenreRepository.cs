using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly DataContext _dataContext;

        public GenreRepository(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<int> GetCountAsync()
        {
            return await _dataContext.Genres.CountAsync();
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            var genreEntities = await _dataContext.Genres.ToListAsync();

            return genreEntities.Select(g => g.ToGenre()).ToList();
        }
    }
}