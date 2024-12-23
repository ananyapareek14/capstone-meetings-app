﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using meetings_server.Models.Domain;
using meetings_server.Models.DTO;
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

    //[HttpGet]
    //[Authorize]
    //public IActionResult GetMeetingsForLoggedInUser()
    //{
    //    // Get the currently logged-in user
    //    var loggedInUser = _userManager.GetUserAsync(User).Result;
    //    if (loggedInUser == null)
    //        return Unauthorized("User not logged in");

    //    // Get all meetings where the logged-in user is an attendee
    //    var meetings = _context.Meetings
    //        .Where(m => m.Attendees.Any(a => a.UserId == loggedInUser.Id))
    //        .Include(m => m.Attendees)
    //        .ThenInclude(a => a.User)
    //        .ToList();

    //    // If no meetings are found for the user
    //    if (!meetings.Any())
    //        return NotFound("No meetings found for the user.");

    //    // Map Meeting entities to MeetingsResponseDto
    //    var response = meetings.Select(meeting => new MeetingsResponseDto
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
    //    }).ToList();

    //    return Ok(response);
    //}

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> SearchMeetings([FromQuery] string? search = null, [FromQuery] string? period = "all")
    {
        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null)
        {
            return Unauthorized("User not logged in");
        }

        var validPeriods = new[] { "all", "past", "present", "future" };
        if (!validPeriods.Contains(period))
        {
            return BadRequest($"Invalid period. Valid values are: {string.Join(", ", validPeriods)}");
        }

        var currentDate = DateTime.UtcNow;

        var query = _context.Meetings
            .Where(m => m.Attendees.Any(a => a.UserId == loggedInUser.Id));

        if (!string.IsNullOrEmpty(search))
        {
            // Convert both sides to lowercase for case-insensitive search
            var searchLower = search.ToLower();
            query = query.Where(m => m.Description.ToLower().Contains(searchLower));
        }

        query = period switch
        {
            "past" => query.Where(m => m.Date < currentDate.Date),
            "present" => query.Where(m => m.Date == currentDate.Date),
            "future" => query.Where(m => m.Date > currentDate.Date),
            _ => query
        };

        var meetings = await query
            .Include(m => m.Attendees)
            .ThenInclude(a => a.User)
            .ToListAsync();

        var response = meetings.Select(meeting => new MeetingsResponseDto
        {
            _id = meeting.Id,
            name = meeting.Name,
            description = meeting.Description,
            date = meeting.Date.ToString("yyyy-MM-dd"),
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

    [HttpPost]
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

        // Add the logged-in user as an attendee (this will always be added)
        meeting.Attendees.Add(new Attendee
        {
            Meeting = meeting,
            UserId = loggedInUser.Id,
            User = loggedInUser
        });

        // Add additional attendees based on the emails provided in the request
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

        // Return the full meeting details
        var response = new
        {
            _id = meeting.Id,
            name = meeting.Name,
            description = meeting.Description,
            date = meeting.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // Format date as string (ISO 8601)
            startTime = new
            {
                hours = meeting.StartTime.Hours,
                minutes = meeting.StartTime.Minutes
            },
            endTime = new
            {
                hours = meeting.EndTime.Hours,
                minutes = meeting.EndTime.Minutes
            },
            attendees = meeting.Attendees.Select(a => new
            {
                userId = a.UserId,
                email = a.User.Email
            }).ToList(),
        };

        return Ok(response);
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> ManageAttendees(Guid id, [FromQuery] string action, [FromQuery] string? email = null)
    {
        // Validate action parameter
        if (string.IsNullOrEmpty(action))
        {
            return BadRequest("Action parameter is required.");
        }

        // Fetch the meeting
        var meeting = await _context.Meetings.Include(m => m.Attendees).FirstOrDefaultAsync(m => m.Id == id);
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
