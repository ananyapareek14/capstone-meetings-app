using Microsoft.AspNetCore.Identity;
namespace meetings_server.Models.Domain;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }

    // navigation property to get meetings for a user (attendee table entries where userId is this user'd Id)
    public ICollection<Attendee> Meetings { get; set; }
}
