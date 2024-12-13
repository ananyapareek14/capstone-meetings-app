﻿namespace meetings_app_server.Models.DTO;

public class MeetingDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public List<string> Attendees { get; set; }
}
