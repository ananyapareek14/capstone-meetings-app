using System.ComponentModel.DataAnnotations;
namespace meetings_server.Models.DTO;

public class RegisterRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
