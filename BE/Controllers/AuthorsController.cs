using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.Authors;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorsService _authorsService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorsService authorsService, ILogger<AuthorsController> logger)
    {
        _authorsService = authorsService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet("{authorId}")]
    public async Task<IActionResult> Get(Guid authorId)
    {
        _logger.LogInformation("Get Author {@AuthorId}", authorId);
        return Ok(await _authorsService.Get(authorId));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAuthors([FromQuery] AuthorParameters authorParameters)
    {
        _logger.LogInformation("Returned all authors from the database");
        return Ok(await _authorsService.GetAuthors(authorParameters));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AuthorsRequest authorRequest)
    {
        _logger.LogInformation("Create Author: " + authorRequest);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await _authorsService.Create(authorRequest.ToAuthor()));
    }

    [HttpPut("{authorId}")]
    public async Task<IActionResult> Update(Guid authorId, [FromBody] AuthorsRequest authorRequest)
    {
        _logger.LogInformation("Update Author: " + authorRequest);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await _authorsService.Update(authorId, authorRequest.ToAuthor()));
    }

    [HttpDelete("{authorId}")]
    public async Task<IActionResult> Delete(Guid authorId)
    {
        _logger.LogInformation("Delete Author with {@authorId}", authorId);

        await _authorsService.Delete(authorId);
        return Ok();
    }
}