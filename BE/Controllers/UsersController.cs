using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.Users;
using BusinessLayer.Models.Requests;
using BusinessLayer.Requests;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersService usersService;

    public UsersController(IUsersService usersService, ILogger<UsersController> logger)
    {
        this.usersService = usersService;
        _logger = logger;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest authenticateRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _logger.LogInformation("Trying to authenticate user: {@email}", authenticateRequest.Email);

        var authenticatedUser =
            await usersService.Authenticate(authenticateRequest.Email, authenticateRequest.Password);

        var cookieOptions = GetRefreshTokenOptions();

        Response.Cookies.Append("refreshToken", authenticatedUser.RefreshToken, cookieOptions);
        return Ok(authenticatedUser);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] UserRequest user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await usersService.Register(user.ToUser());

        _logger.LogInformation("Registered user: {@email}", user.EmailAddress);
        return Ok(new {message = $"Registered user: {user.EmailAddress}"});
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken is null) return BadRequest(new {message = "Token is required"});

        var authenticatedUser = await usersService.RefreshToken(User);
        var cookieOptions = GetRefreshTokenOptions();

        Response.Cookies.Append("refreshToken", authenticatedUser.RefreshToken, cookieOptions);

        return Ok(authenticatedUser);
    }

    [HttpPut("[action]/{userId}")]
    [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
    public async Task<IActionResult> Approve(Guid userId)
    {
        var user = await usersService.ApproveUser(userId, User);
        return Ok(user.ToUserResponse());
    }

    [HttpPut("[action]/{userId}")]
    [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
    public async Task<IActionResult> Reject(Guid userId)
    {
        await usersService.RejectUser(userId, User);
        return Ok();
    }

    [HttpGet("customers/get-pending")]
    [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
    public async Task<IActionResult> GetPendingCustomers()
    {
        var pendingCustomers = await usersService.GetPendingCustomers();
        return Ok(pendingCustomers.Select(u => u.ToUserResponse()));
    }

    [HttpGet("get-pending")]
    [Authorize(Policy = Policies.RequireAdministratorRole)]
    public async Task<IActionResult> GetPendingUsers()
    {
        var pendingUsers = await usersService.GetPendingUsers();
        return Ok(pendingUsers.Select(u => u.ToUserResponse()));
    }

    [HttpGet("get/{userId}")]
    [Authorize(Policy = Policies.RequireCustomerRole)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var user = await usersService.GetUserById(userId);
        return Ok(user.ToUserResponse());
    }

    [HttpGet("get-librarian/{librarianId}")]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public async Task<IActionResult> GetLibrarianById(Guid librarianId)
    {
        var librarian = await usersService.GetLibrarianById(librarianId);
        return Ok(librarian.ToUserResponse());
    }

    private CookieOptions GetRefreshTokenOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }

    [AllowAnonymous]
    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var numberOfUsers = await usersService.GetCount();
        return Ok(numberOfUsers);
    }

    [HttpGet]
    [Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
    public async Task<IActionResult> GetAllUsers([FromQuery] string filter)
    {
        var users = await usersService.GetUsers(filter);
        return Ok(users.Select(u => u.ToUserResponse()));
    }
}