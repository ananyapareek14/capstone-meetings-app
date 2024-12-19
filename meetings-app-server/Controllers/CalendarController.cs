//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;
//using meetings_server.Models.Domain;
//using meetings_server.Models.DTO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using meetings_server.Data;
//using Microsoft.AspNetCore.Authorization;

//namespace meetings_server.Controllers;

//[ApiController]
//[Route("api/calendar")]
//public class CalendarController : ControllerBase
//{
//    private readonly ApplicationDbContext _context;
//    private readonly UserManager<ApplicationUser> _userManager;

//    public CalendarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//    {
//        _context = context;
//        _userManager = userManager;
//    }

//    //[HttpGet]
//    //public async Task<IActionResult> GetMeetingsByDate([FromQuery] string date)
//    //{
//    //    // Validate the date format
//    //    if (!DateTime.TryParse(date, out DateTime requestedDate))
//    //    {
//    //        return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
//    //    }

//    //    // Query meetings matching the requested date
//    //    var meetings = await _context.Meetings
//    //        .Include(m => m.Attendees)
//    //        .ThenInclude(a => a.User)
//    //        .Where(m => m.Date.Date == requestedDate.Date) // Match the date
//    //        .ToListAsync();

//    //    // Format the response
//    //    var result = meetings.Select(m => new
//    //    {
//    //        startTime = new
//    //        {
//    //            hours = m.StartTime.Hours,
//    //            minutes = m.StartTime.Minutes
//    //        },
//    //        endTime = new
//    //        {
//    //            hours = m.EndTime.Hours,
//    //            minutes = m.EndTime.Minutes
//    //        },
//    //        _id = m.Id,
//    //        name = m.Name,
//    //        description = m.Description,
//    //        date = m.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // ISO Date Format
//    //        attendees = m.Attendees.Select(a => new
//    //        {
//    //            userId = a.UserId,
//    //            email = a.User.Email
//    //        }).ToList()
//    //    });

//    //    return Ok(result);
//    //}

//    [HttpGet]
//    [Authorize]
//    public async Task<IActionResult> GetMeetingsByDate([FromQuery] string date)
//    {
//        // Validate the date format
//        if (!DateTime.TryParse(date, out DateTime requestedDate))
//        {
//            return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
//        }

//        // Get the logged-in user's ID
//        var userId = _userManager.GetUserId(User); // Retrieve the ID of the logged-in user

//        if (string.IsNullOrEmpty(userId))
//        {
//            return Unauthorized("User not found.");
//        }

//        // Query meetings matching the requested date and associated with the logged-in user
//        var meetings = await _context.Meetings
//            .Include(m => m.Attendees)
//            .ThenInclude(a => a.User)
//            .Where(m => m.Date.Date == requestedDate.Date &&
//                        m.Attendees.Any(a => a.UserId == userId)) // Filter by attendee userId
//            .ToListAsync();

//        // Format the response
//        var result = meetings.Select(m => new
//        {
//            startTime = new
//            {
//                hours = m.StartTime.Hours,
//                minutes = m.StartTime.Minutes
//            },
//            endTime = new
//            {
//                hours = m.EndTime.Hours,
//                minutes = m.EndTime.Minutes
//            },
//            _id = m.Id,
//            name = m.Name,
//            description = m.Description,
//            date = m.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // ISO Date Format
//            attendees = m.Attendees.Select(a => new
//            {
//                userId = a.UserId,
//                email = a.User.Email
//            }).ToList()
//        });

//        return Ok(result);
//    }

//}

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
[Route("api/calendar")]
public class CalendarController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CalendarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetMeetingsByDate([FromQuery] string date)
    {
        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null)
            return Unauthorized("User not logged in");

        // Validate the date format
        if (!DateTime.TryParse(date, out DateTime requestedDate))
        {
            return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
        }

        // Query meetings where the logged-in user is an attendee
        var meetings = await _context.Meetings
            .Include(m => m.Attendees)
            .ThenInclude(a => a.User)
            .Where(m => m.Date.Date == requestedDate.Date) // Match the date
            .Where(m => m.Attendees.Any(a => a.UserId == loggedInUser.Id)) // Filter by logged-in user's attendance
            .ToListAsync();

        // Format the response
        var result = meetings.Select(m => new
        {
            startTime = new
            {
                hours = m.StartTime.Hours,
                minutes = m.StartTime.Minutes
            },
            endTime = new
            {
                hours = m.EndTime.Hours,
                minutes = m.EndTime.Minutes
            },
            _id = m.Id,
            name = m.Name,
            description = m.Description,
            date = m.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // ISO Date Format
            attendees = m.Attendees.Select(a => new
            {
                userId = a.UserId,
                email = a.User.Email
            }).ToList()
        });

        return Ok(result);
    }

}