namespace meetings_server.Models.DTO;

public class LoginResponseDto
{
    public string token { get; set; }
    public string email { get; set; }
    public string name { get; set; }
}
