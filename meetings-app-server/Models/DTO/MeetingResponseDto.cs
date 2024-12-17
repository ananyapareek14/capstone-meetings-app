namespace meetings_server.Models.DTO;

public class AttendeeResponseDto
{
    public string userId { get; set; }
    public string email { get; set; }
}

public class MeetingsResponseDto
{
    public Guid _id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string date { get; set; }
    public TimeDto startTime { get; set; }
    public TimeDto endTime { get; set; }
    public List<AttendeeResponseDto> attendees { get; set; }
}
