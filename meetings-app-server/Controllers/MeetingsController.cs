using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using meetings_app_server.Models.DTO;
using meetings_app_server.Models.Domain;
using meetings_app_server.Repositories;

namespace meetings_app_server.Controllers;

[ApiController]
[Route("api/meetings")]
[Authorize]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IUserRepository _userRepository;

    public MeetingsController(IMeetingRepository meetingRepository, IUserRepository userRepository)
    {
        _meetingRepository = meetingRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> SearchMeetings([FromQuery] string search)
    {
        var meetings = await _meetingRepository.SearchMeetings(search);
        return Ok(meetings);
    }

    [HttpPost]
    public async Task<IActionResult> AddMeeting([FromBody] MeetingDto meetingDto)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId"));

        var meeting = new Meeting
        {
            MeetingId = Guid.NewGuid(),
            Name = meetingDto.Name,
            Description = meetingDto.Description,
            Date = meetingDto.Date,
            StartTime = meetingDto.StartTime,
            EndTime = meetingDto.EndTime
        };

        await _meetingRepository.AddMeeting(meeting);
        await _meetingRepository.AddAttendee(meeting.MeetingId, userId);

        foreach (var attendeeEmail in meetingDto.Attendees)
        {
            var attendee = await _userRepository.GetAllUsers()
                .ContinueWith(t => t.Result.FirstOrDefault(u => u.Email == attendeeEmail));
            if (attendee != null)
            {
                await _meetingRepository.AddAttendee(meeting.MeetingId, attendee.UserId);
            }
        }

        return Ok(meeting);
    }

    [HttpPatch("{meetingId}")]
    public async Task<IActionResult> UpdateAttendees(Guid meetingId, [FromQuery] string action, [FromQuery] string email = null)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId"));

        if (action == "add_attendee")
        {
            var attendee = await _userRepository.GetAllUsers()
                .ContinueWith(t => t.Result.FirstOrDefault(u => u.Email == email));
            if (attendee == null)
            {
                return NotFound(new { message = "User not found" });
            }

            await _meetingRepository.AddAttendee(meetingId, attendee.UserId);
            return Ok(new { message = "Attendee added" });
        }

        if (action == "remove_attendee")
        {
            await _meetingRepository.RemoveAttendee(meetingId, userId);
            return Ok(new { message = "You have been removed from the meeting" });
        }

        return BadRequest(new { message = "Invalid action" });
    }
}
