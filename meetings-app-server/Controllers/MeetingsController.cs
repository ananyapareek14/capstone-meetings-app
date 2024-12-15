using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using meetings_server.Models.Domain;
using meetings_server.Models.DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using meetings_server.Data;

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

    // GET: api/meetings?search=demo&period=past
    //[HttpGet]
    //public async Task<IActionResult> GetMeetings([FromQuery] string search, [FromQuery] string period)
    //{
    //    //var loggedInUser = await _userManager.GetUserAsync(User);
    //    //if (loggedInUser == null)
    //    //{
    //    //    return Unauthorized("User not logged in");
    //    //}

    //    var query = _context.Meetings
    //        .Where(m => m.Attendees.Any(a => a.UserId == loggedInUser.Id));

    //    if (!string.IsNullOrEmpty(search))
    //    {
    //        query = query.Where(m => m.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
    //    }

    //    switch (period.ToLower())
    //    {
    //        case "past":
    //            query = query.Where(m => m.Date < DateTime.Now);
    //            break;
    //        case "present":
    //            query = query.Where(m => m.Date == DateTime.Now.Date && m.StartTime <= DateTime.Now.TimeOfDay && m.EndTime >= DateTime.Now.TimeOfDay);
    //            break;
    //        case "future":
    //            query = query.Where(m => m.Date > DateTime.Now);
    //            break;
    //        default:
    //            break;
    //    }

    //    var meetings = await query.ToListAsync();

    //    return Ok(meetings);
    //}

    //[HttpGet]
    //public async Task<IActionResult> GetMeetings()
    //{
    //    // Retrieve all meetings from the database
    //    var meetings = await _context.Meetings.ToListAsync();

    //    // Return the meetings as a response
    //    return Ok(meetings);
    //}

    // GET: api/meetings
    [HttpGet]
    public async Task<IActionResult> GetMeetings()
    {
        var meetings = await _context.Meetings
            //.Include(m => m.Attendees)  
            //.ThenInclude(a => a.User)  
            .ToListAsync();

        return Ok(meetings);
    }


    // POST: api/meetings
    [HttpPost]
    public async Task<IActionResult> AddMeeting([FromBody] MeetingRequestDto request)
    {
        //var loggedInUser = await _userManager.GetUserAsync(User);
        //if (loggedInUser == null)
        //{
        //    return Unauthorized("User not logged in");
        //}

        // Create the meeting object
        var meeting = new Meeting
        {
            Name = request.Name,
            Description = request.Description,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Attendees = new List<Attendee>() // Initialize Attendees as a new list
        };

        // Add the logged-in user as an attendee
        //meeting.Attendees.Add(new Attendee
        //{
        //    Meeting = meeting,
        //    UserId = loggedInUser.Id,
        //    User = loggedInUser
        //});

        // Add attendees based on the emails provided in the request
        if (request.Attendees != null && request.Attendees.Any())
        {
            foreach (var email in request.Attendees)
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

        // Add the meeting to the database
        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Meeting created successfully", MeetingId = meeting.Id });
    }


    //// PATCH: api/meetings/{meetingId}?action=add_attendee&email=example@example.com
    //[HttpPatch("{meetingId}")]
    //public async Task<IActionResult> AddAttendee(int meetingId, [FromQuery] string action, [FromQuery] string email, [FromQuery] string userId)
    //{
    //    if (action != "add_attendee")
    //    {
    //        return BadRequest("Invalid action");
    //    }

    //    var meeting = await _context.Meetings.Include(m => m.Attendees).FirstOrDefaultAsync(m => m.Id == meetingId);
    //    if (meeting == null)
    //    {
    //        return NotFound("Meeting not found");
    //    }

    //    ApplicationUser attendee;
    //    if (!string.IsNullOrEmpty(userId))
    //    {
    //        attendee = await _userManager.FindByIdAsync(userId);
    //    }
    //    else if (!string.IsNullOrEmpty(email))
    //    {
    //        attendee = await _userManager.FindByEmailAsync(email);
    //    }
    //    else
    //    {
    //        return BadRequest("Either userId or email must be provided");
    //    }

    //    if (attendee == null)
    //    {
    //        return NotFound("User not found");
    //    }

    //    if (!meeting.Attendees.Any(a => a.UserId == attendee.Id))
    //    {
    //        meeting.Attendees.Add(new Attendee
    //        {
    //            Meeting = meeting,
    //            UserId = attendee.Id,
    //            User = attendee
    //        });

    //        await _context.SaveChangesAsync();
    //    }

    //    return Ok(new { Message = "Attendee added successfully" });
    //}

    //// PATCH: api/meetings/{meetingId}?action=remove_attendee
    //[HttpPatch("{meetingId}")]
    //public async Task<IActionResult> RemoveAttendee(int meetingId, [FromQuery] string action)
    //{
    //    if (action != "remove_attendee")
    //    {
    //        return BadRequest("Invalid action");
    //    }

    //    var loggedInUser = await _userManager.GetUserAsync(User);
    //    if (loggedInUser == null)
    //    {
    //        return Unauthorized("User not logged in");
    //    }

    //    var meeting = await _context.Meetings.Include(m => m.Attendees)
    //        .FirstOrDefaultAsync(m => m.Id == meetingId);
    //    if (meeting == null)
    //    {
    //        return NotFound("Meeting not found");
    //    }

    //    var attendee = meeting.Attendees.FirstOrDefault(a => a.UserId == loggedInUser.Id);
    //    if (attendee == null)
    //    {
    //        return BadRequest("User is not an attendee of this meeting");
    //    }

    //    meeting.Attendees.Remove(attendee);
    //    await _context.SaveChangesAsync();

    //    return Ok(new { Message = "You have been removed from the meeting" });
    //}
}
