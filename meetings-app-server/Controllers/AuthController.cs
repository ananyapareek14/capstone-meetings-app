using meetings_server.Models.Domain;
using meetings_server.Models.DTO;
using meetings_server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenRepository _tokenRepository;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, Name = request.Name };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            return Ok(new { Message = "User registered successfully" });

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var token = _tokenRepository.CreateJWTToken(user);
            return Ok(new { token, user.Email, user.Id });
        }

        return Unauthorized("Invalid credentials");
    }

    [HttpGet("users")]
    [Authorize(Roles = "Reader,Writer")]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.Select(user => new
        {
            user.Id,
            user.Email,
            user.UserName
        }).ToList();

        return Ok(users);
    }
}
