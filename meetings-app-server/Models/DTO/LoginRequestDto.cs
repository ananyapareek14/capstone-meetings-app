using System.ComponentModel.DataAnnotations;

namespace meetings_server.Models.DTO;

public class LoginRequestDto
{
    [Required] public string Email{  get; set; }
    [Required] public string Password { get; set; }
}
