namespace meetings_server.Models.DTO;

public class TimeDto
{
    public int hours { get; set; }
    public int minutes { get; set; }
}

public class MeetingRequestDto
{
    public string name { get; set; }
    public string description { get; set; }
    public DateTime date { get; set; }
    public TimeDto startTime { get; set; }
    public TimeDto endTime { get; set; }
    public List<string> attendees { get; set; }
}
