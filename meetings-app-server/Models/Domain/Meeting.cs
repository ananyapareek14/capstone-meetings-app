namespace meetings_app_server.Models.Domain;

public class Meeting
{
    public Guid MeetingId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ICollection<MeetingAttendee> Attendees { get; set; } = new List<MeetingAttendee>();
}
