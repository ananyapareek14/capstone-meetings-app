using meetings_app_server.Models.Domain;
namespace meetings_app_server.Repositories;

public interface IMeetingRepository
{
    Task<Meeting> AddMeeting(Meeting meeting);
    Task<IEnumerable<Meeting>> SearchMeetings(string search);
    Task AddAttendee(Guid meetingId, Guid userId);
    Task RemoveAttendee(Guid meetingId, Guid userId);
}
