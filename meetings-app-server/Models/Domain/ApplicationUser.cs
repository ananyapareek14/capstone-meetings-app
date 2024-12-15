using Microsoft.AspNetCore.Identity;
namespace meetings_server.Models.Domain;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}
