using meetings_server.Models.Domain;
using System;
using System.Collections.Generic;

namespace meetings_server.Models.Domain;

public class Meeting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public ICollection<Attendee> Attendees { get; set; }
}
