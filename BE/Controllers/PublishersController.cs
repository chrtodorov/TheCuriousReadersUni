using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.Publishers;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
[Route("api/[controller]")]
[ApiController]
public class PublishersController : ControllerBase
{
    private readonly ILogger<PublishersController> _logger;
    private readonly IPublishersService _publishersService;

    public PublishersController(IPublishersService publishersService, ILogger<PublishersController> logger)
    {
        _publishersService = publishersService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet("{publisherId}")]
    public async Task<IActionResult> Get(Guid publisherId)
    {
        _logger.LogInformation("Get Publisher {@PublisherId}", publisherId);
        return Ok(await _publishersService.Get(publisherId));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PublisherParameters parameters)
    {
        _logger.LogInformation("Get All Publishers");
        return Ok(await _publishersService.GetAll(parameters));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PublisherRequest publisherRequest)
    {
        _logger.LogInformation("Create Publisher" + publisherRequest);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await _publishersService.Create(publisherRequest.ToPublisher()));
    }

    [HttpPut("{publisherId}")]
    public async Task<IActionResult> Update(Guid publisherId, [FromBody] PublisherRequest publisherRequest)
    {
        _logger.LogInformation("Update Publisher" + publisherRequest);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await _publishersService.Update(publisherId, publisherRequest.ToPublisher()));
    }

    [HttpDelete("{publisherId}")]
    public async Task<IActionResult> Delete(Guid publisherId)
    {
        _logger.LogInformation("Delete Publisher with {@PublisherId}", publisherId);

        await _publishersService.Delete(publisherId);
        return Ok();
    }
}