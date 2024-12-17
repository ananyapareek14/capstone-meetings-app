using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using meetings_server.Models.Domain;
using meetings_server.Models.DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using meetings_server.Data;
using Microsoft.AspNetCore.Authorization;

namespace meetings_server.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.Select(user => new
        {
            _id = user.Id,
            name = user.Name,
            email = user.Email
        }).ToList();

        return Ok(users);
    }
}
