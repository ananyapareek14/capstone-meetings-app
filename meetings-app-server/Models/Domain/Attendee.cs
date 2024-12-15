namespace meetings_server.Models.Domain;

public class Attendee
{
    public int MeetingId { get; set; }
    public Meeting Meeting { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
