using System.ComponentModel.DataAnnotations;
namespace meetings_server.Models.DTO;

public class RegisterRequestDto
{
    public string name { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}
