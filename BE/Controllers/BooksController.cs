using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.Books;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly ILogger<BooksController> _logger;

    public BooksController(IBooksService booksService, ILogger<BooksController> logger)
    {
        _booksService = booksService;
        _logger = logger;
    }

        [HttpGet("{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Book))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid bookId)
        {
            _logger.LogInformation("Get Book {@BookId}", bookId);

            var result = await _booksService.Get(bookId);

            if (result is null)
                return NotFound("Book with such Id is not found!");
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<Book>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBooks([FromQuery] BookParameters booksParameters)
        {
            _logger.LogInformation($"Returned all books from database");
            return Ok(await _booksService.GetBooks(booksParameters));
        }

        [HttpGet("latestbooks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Book>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLatest()
        {
            var books = await _booksService.GetLatest();

            if (books.Count==0)
                return NotFound("No books found");

            _logger.LogInformation($"Returned {books.Count} books from database");

            return Ok(books);
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Book))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] BookRequest bookRequest)
        {
            _logger.LogInformation("Create Book: " + bookRequest.ToString());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await _booksService.Create(bookRequest.ToBook()));
    }

        [HttpPut("{bookId}")]
        [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Book))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid bookId, [FromBody] BookRequest bookRequest)
        {
            _logger.LogInformation("Update Book: " + bookRequest.ToString());

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok(await _booksService.Update(bookId, bookRequest.ToBook()));
    }

        [HttpDelete("{bookId}")]
        [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid bookId)
        {
            _logger.LogInformation("Delete Book with {@bookId}", bookId);

            try
            {
                await _booksService.Delete(bookId);
                return Ok();
            }
            catch (ArgumentNullException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetNumber()
        {
            var numberOfBooks = await _booksService.GetNumber();
            return Ok(numberOfBooks);
        }

        [HttpPut("{bookId}/status")]
        [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
        public async Task<IActionResult> MakeUnavailable(Guid bookId)
        {
            try
            {
                await _booksService.MakeUnavailable(bookId);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("[action]")]
        [Authorize(Policy = Policies.RequireCustomerRole)]
        public IActionResult Read([FromQuery] PagingParameters pagingParameters)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value!);
            var books = _booksService.GetReadBooks(userId, pagingParameters);

            return Ok(books);
        }

    }
}
