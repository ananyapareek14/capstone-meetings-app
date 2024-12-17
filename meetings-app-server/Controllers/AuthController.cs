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
        var user = new ApplicationUser { UserName = request.email, Email = request.email, Name = request.name };
        var result = await _userManager.CreateAsync(user, request.password);

        if (result.Succeeded)
            return Ok(new { Message = "User registered successfully" });

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.email);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.password))
        {
            var token = _tokenRepository.CreateJWTToken(user);
            return Ok(new { token, user.Email, user.Id });
        }

        return Unauthorized("Invalid credentials");
    }

    [HttpGet("users")]
    [Authorize]
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
