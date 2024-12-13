using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using meetings_app_server.Repositories;

namespace meetings_app_server.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsers();
        return Ok(users.Select(u => new {
            u.UserId,
            u.Name,
            u.Email
        }));
    }
}