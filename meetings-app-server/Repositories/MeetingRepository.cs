using meetings_app_server.Models.Domain;
using meetings_app_server.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace meetings_app_server.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly ApplicationDbContext _db;

    public MeetingRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Meeting> AddMeeting(Meeting meeting)
    {
        _db.Meetings.Add(meeting);
        await _db.SaveChangesAsync();
        return meeting;
    }

    public async Task<IEnumerable<Meeting>> SearchMeetings(string search)
    {
        return await _db.Meetings
            .Where(m => m.Description.Contains(search))
            .ToListAsync();
    }

    public async Task AddAttendee(Guid meetingId, Guid userId)
    {
        _db.MeetingAttendees.Add(new MeetingAttendee
        {
            MeetingId = meetingId,
            UserId = userId
        });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAttendee(Guid meetingId, Guid userId)
    {
        var attendee = await _db.MeetingAttendees
            .FirstOrDefaultAsync(ma => ma.MeetingId == meetingId && ma.UserId == userId);
        if (attendee != null)
        {
            _db.MeetingAttendees.Remove(attendee);
            await _db.SaveChangesAsync();
        }
    }
}