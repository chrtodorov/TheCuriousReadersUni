using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _genreService.GetGenresAsync());
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            return Ok(await _genreService.GetCountAsync());
        }
    }
}