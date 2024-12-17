//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using meetings_server.Data;

//[HttpGet]
//public async Task<IActionResult> GetMeetingsByDate([FromQuery] string date)
//{
//    // Validate the date format
//    if (!DateTime.TryParse(date, out DateTime requestedDate))
//    {
//        return BadRequest("Invalid date format. Please use yyyy-MM-dd.");
//    }

//    // Query meetings matching the requested date
//    var meetings = await _context.Meetings
//        .Include(m => m.Attendees)
//        .ThenInclude(a => a.User)
//        .Where(m => m.Date.Date == requestedDate.Date) // Match the date
//        .ToListAsync();

//    // Format the response
//    var result = meetings.Select(m => new
//    {
//        startTime = new
//        {
//            hours = m.StartTime.Hours,
//            minutes = m.StartTime.Minutes
//        },
//        endTime = new
//        {
//            hours = m.EndTime.Hours,
//            minutes = m.EndTime.Minutes
//        },
//        _id = m.Id,
//        name = m.Name,
//        description = m.Description,
//        date = m.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), // ISO Date Format
//        attendees = m.Attendees.Select(a => new
//        {
//            userId = a.UserId,
//            email = a.User.Email
//        }).ToList()
//    });

//    return Ok(result);
//}
