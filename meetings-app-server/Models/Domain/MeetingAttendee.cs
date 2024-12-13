namespace meetings_app_server.Models.Domain;

public class MeetingAttendee
{
    public Guid MeetingId { get; set; }
    public Meeting Meeting { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
