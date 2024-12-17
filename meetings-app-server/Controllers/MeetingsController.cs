using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using meetings_server.Models.Domain;
using meetings_server.Models.DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using meetings_server.Data;
using Microsoft.AspNetCore.Authorization;

namespace meetings_server.Controllers;

[ApiController]
[Route("api/meetings")]
public class MeetingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MeetingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetMeetingsForLoggedInUser()
    {
        // Get the currently logged-in user
        var loggedInUser = _userManager.GetUserAsync(User).Result;
        if (loggedInUser == null)
            return Unauthorized("User not logged in");

        // Get all meetings where the logged-in user is an attendee
        var meetings = _context.Meetings
            .Where(m => m.Attendees.Any(a => a.UserId == loggedInUser.Id))
            .Include(m => m.Attendees)
            .ThenInclude(a => a.User)
            .ToList();

        // If no meetings are found for the user
        if (!meetings.Any())
            return NotFound("No meetings found for the user.");

        // Map Meeting entities to MeetingsResponseDto
        var response = meetings.Select(meeting => new MeetingsResponseDto
        {
            _id = meeting.Id,
            name = meeting.Name,
            description = meeting.Description,
            date = meeting.Date.ToString("yyyy-MM-dd"), // Format date as string
            startTime = new TimeDto
            {
                hours = meeting.StartTime.Hours,
                minutes = meeting.StartTime.Minutes
            },
            endTime = new TimeDto
            {
                hours = meeting.EndTime.Hours,
                minutes = meeting.EndTime.Minutes
            },
            attendees = meeting.Attendees.Select(a => new AttendeeResponseDto
            {
                userId = a.UserId,
                email = a.User.Email
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    //[HttpGet("{id}")]
    //[Authorize]
    //public IActionResult GetMeeting(Guid id)
    //{
    //    var meeting = _context.Meetings
    //        .Include(m => m.Attendees)
    //        .ThenInclude(a => a.User)
    //        .FirstOrDefault(m => m.Id == id);

    //    if (meeting == null)
    //        return NotFound();

    //    // Map Meeting entity to MeetingsResponseDto
    //    var response = new MeetingsResponseDto
    //    {
    //        _id = meeting.Id,
    //        name = meeting.Name,
    //        description = meeting.Description,
    //        date = meeting.Date.ToString("yyyy-MM-dd"), // Format date as string
    //        startTime = new TimeDto
    //        {
    //            hours = meeting.StartTime.Hours,
    //            minutes = meeting.StartTime.Minutes
    //        },
    //        endTime = new TimeDto
    //        {
    //            hours = meeting.EndTime.Hours,
    //            minutes = meeting.EndTime.Minutes
    //        },
    //        attendees = meeting.Attendees.Select(a => new AttendeeResponseDto
    //        {
    //            userId = a.UserId,
    //            email = a.User.Email
    //        }).ToList()
    //    };

    //    return Ok(response);
    //}


    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddMeeting([FromBody] MeetingRequestDto request)
    {
        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null)
        {
            return Unauthorized("User not logged in");
        }

        // Validate time inputs
        if (request.startTime == null || request.endTime == null)
        {
            return BadRequest("StartTime and EndTime are required.");
        }

        // Convert TimeDto to TimeSpan
        var startTime = new TimeSpan(request.startTime.hours, request.startTime.minutes, 0);
        var endTime = new TimeSpan(request.endTime.hours, request.endTime.minutes, 0);

        // Create the meeting object
        var meeting = new Meeting
        {
            Name = request.name,
            Description = request.description,
            Date = request.date,
            StartTime = startTime,
            EndTime = endTime,
            Attendees = new List<Attendee>() // Initialize Attendees as a new list
        };

        // Add the logged-in user as an attendee
        meeting.Attendees.Add(new Attendee
        {
            Meeting = meeting,
            UserId = loggedInUser.Id,
            User = loggedInUser
        });

        // Add attendees based on the emails provided in the request
        if (request.attendees != null && request.attendees.Any())
        {
            foreach (var email in request.attendees)
            {
                var attendee = await _userManager.FindByEmailAsync(email);
                if (attendee != null)
                {
                    meeting.Attendees.Add(new Attendee
                    {
                        Meeting = meeting,
                        UserId = attendee.Id,
                        User = attendee
                    });
                }
                else
                {
                    return BadRequest($"User with email {email} not found");
                }
            }
        }

        // Add meeting to database
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Meeting added successfully", meetingId = meeting.Id });
    }


    [HttpPatch("{meetingId}")]
    [Authorize]
    public async Task<IActionResult> ManageAttendees(Guid meetingId, [FromQuery] string action, [FromQuery] string? email = null)
    {
        // Validate action parameter
        if (string.IsNullOrEmpty(action))
        {
            return BadRequest("Action parameter is required.");
        }

        // Fetch the meeting
        var meeting = await _context.Meetings.Include(m => m.Attendees)
                                             .FirstOrDefaultAsync(m => m.Id == meetingId);
        if (meeting == null)
        {
            return NotFound("Meeting not found.");
        }

        // Get the logged-in user
        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null)
        {
            return Unauthorized("User not logged in.");
        }

        // Action: add_attendee
        if (action == "add_attendee")
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required to add an attendee.");
            }

            var attendeeUser = await _userManager.FindByEmailAsync(email);
            if (attendeeUser == null)
            {
                return NotFound("User with the provided email not found.");
            }

            // Check if the user is already an attendee
            if (meeting.Attendees.Any(a => a.UserId == attendeeUser.Id))
            {
                return BadRequest("User is already an attendee of this meeting.");
            }

            // Add the attendee
            meeting.Attendees.Add(new Attendee
            {
                Meeting = meeting,
                UserId = attendeeUser.Id,
                User = attendeeUser
            });

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Attendee added successfully." });
        }

        // Action: remove_attendee
        else if (action == "remove_attendee")
        {
            // Find the logged-in user's attendee record
            var attendee = meeting.Attendees.FirstOrDefault(a => a.UserId == loggedInUser.Id);
            if (attendee == null)
            {
                return BadRequest("You are not an attendee of this meeting.");
            }

            // Remove the logged-in user from the meeting
            meeting.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "You have been removed from the meeting." });
        }

        // Invalid action
        else
        {
            return BadRequest("Invalid action. Valid actions are 'add_attendee' or 'remove_attendee'.");
        }
    }


}
