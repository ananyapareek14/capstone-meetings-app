//using Microsoft.AspNetCore.Mvc;
//using meetings_app_server.Models.DTO;
//using meetings_app_server.Models.Domain;
//using meetings_app_server.Repositories;
//using meetings_app_server.Services;

//namespace meetings_app_server.Controllers;

////[ApiController]
////[Route("api/auth")]
////public class AuthController : ControllerBase
////{
////    private readonly IUserRepository _userRepository;
////    private readonly JwtService _jwtService;

////    public AuthController(IUserRepository userRepository, JwtService jwtService)
////    {
////        _userRepository = userRepository;
////        _jwtService = jwtService;
////    }

////    [HttpPost("register")]
////    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
////    {
////        var user = new User
////        {
////            UserId = Guid.NewGuid(),
////            Name = registerDto.Name,
////            Email = registerDto.Email,
////            PasswordHash = registerDto.PasswordHash,
////            SessionKey = null
////        };

////        //var result = await _userRepository.Register(user);
////        //return Ok(new { message = "User registered successfully" });
////        var result = await _userRepository.Register(user);

////        if (result)
////        {
////            return Ok(new { Message = "User registered successfully." });
////        }

////        return BadRequest(new { Message = "Error registering user." });
////    }

////    //[HttpPost("login")]
////    //public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
////    //{
////    //    var user = await _userRepository.Login(loginDto.Email, loginDto.Password);
////    //    if (user == null)
////    //    {
////    //        return Unauthorized(new { message = "Invalid credentials" });
////    //    }

////    //    var token = _jwtService.GenerateToken(user.Email, user.UserId);
////    //    return Ok(new { token });
////    //}
////    [HttpPost("login")]
////    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
////    {
////        var user = await _userRepository.Login(loginDto.Email, loginDto.Password);
////        if (user == null)
////        {
////            return Unauthorized(new { message = "Invalid credentials" });
////        }

////        // Generate a unique key for the session
////        var dynamicKey = Guid.NewGuid().ToString();

////        // Store the dynamic key in a database or cache (e.g., Redis)
////        await _userRepository.SaveUserSessionKey(user.UserId, dynamicKey);

////        var token = _jwtService.GenerateToken(user.Email, user.UserId, dynamicKey);
////        return Ok(new { token });
////    }

////}

//[ApiController]
//[Route("api/auth")]
//public class AuthController : ControllerBase
//{
//    private readonly IUserRepository _userRepository;
//    private readonly JwtService _jwtService;

//    public AuthController(IUserRepository userRepository, JwtService jwtService)
//    {
//        _userRepository = userRepository;
//        _jwtService = jwtService;
//    }

//    [HttpPost("register")]
//    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
//    {
//        // Hash the password before saving
//        var hashedPassword = HashPassword(registerDto.Password);

//        var user = new User
//        {
//            UserId = Guid.NewGuid(),
//            Name = registerDto.Name,
//            Email = registerDto.Email,
//            PasswordHash = hashedPassword,
//            SessionKey = null // Ensure SessionKey is null for new users
//        };

//        await _userRepository.Register(user);
//        return Ok(new { message = "User registered successfully" });
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
//    {
//        var user = await _userRepository.Login(loginDto.Email, loginDto.Password);
//        if (user == null)
//        {
//            return Unauthorized(new { message = "Invalid credentials" });
//        }

//        // Generate a unique session key
//        var dynamicKey = Guid.NewGuid().ToString();

//        // Store the session key in the database or cache
//        await _userRepository.SaveUserSessionKey(user.UserId, dynamicKey);

//        var token = _jwtService.GenerateToken(user.Email, user.UserId, dynamicKey);
//        return Ok(new { token });
//    }

//    // Helper method to hash the password
//    private string HashPassword(string password)
//    {
//        using (var sha256 = System.Security.Cryptography.SHA256.Create())
//        {
//            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
//            return Convert.ToBase64String(hashedBytes);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using meetings_app_server.Models.DTO;
using meetings_app_server.Models.Domain;
using meetings_app_server.Repositories;
using meetings_app_server.Services;
using System.Security.Cryptography;
using System.Text;

namespace meetings_app_server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthController(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            // Hash the password before saving
            var hashedPassword = HashPassword(registerDto.Password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                SessionKey = null // Ensure SessionKey is null for new users
            };

            await _userRepository.Register(user);
            return Ok(new { message = "User registered successfully" });
        }

        // Login user and generate token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.Login(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Generate a unique session key
            var dynamicKey = Guid.NewGuid().ToString();

            // Store the session key in the database or cache
            await _userRepository.SaveUserSessionKey(user.UserId, dynamicKey);

            // Generate JWT token with dynamic session key
            var token = _jwtService.GenerateToken(user.Email, user.UserId, dynamicKey);
            return Ok(new { token });
        }

        // Helper method to hash password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
