using genzcoders.Data;
using genzcoders.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace genzcoders.Controllers;

[ApiController]
[Route("api/instructor")]
[Authorize]
public class InstructorController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<InstructorController> _logger;

    public InstructorController(AppDbContext db, ILogger<InstructorController> logger)
    {
        _db = db;
        _logger = logger;
    }

    private async Task<bool> IsInstructorAsync(CancellationToken cancellationToken = default)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountIdClaim) || !long.TryParse(accountIdClaim, out long accountId))
            return false;

        var account = await _db.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        return account != null && account.Role.RoleName.Equals(RoleHelper.Instructor, StringComparison.OrdinalIgnoreCase);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard(CancellationToken cancellationToken = default)
    {
        if (!await IsInstructorAsync(cancellationToken))
            return Forbid("Only instructors can access dashboard.");

        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var accountId = long.Parse(accountIdClaim!);

        // Get instructor's course rounds
        var myCourseRounds = await _db.CourseRounds
            .Include(cr => cr.Course)
            .Include(cr => cr.Status)
            .Where(cr => cr.MainInstructorId == accountId)
            .CountAsync(cancellationToken);

        // Get total students in instructor's rounds
        var totalStudents = await _db.CourseRoundStudents
            .Where(crs => crs.CourseRound.MainInstructorId == accountId)
            .CountAsync(cancellationToken);

        // Get active course rounds
        var activeCourseRounds = await _db.CourseRounds
            .Where(cr => cr.MainInstructorId == accountId && cr.Status.StatusName.ToLower().Contains("active"))
            .CountAsync(cancellationToken);

        // Get materials count
        var totalMaterials = await _db.Materials
            .Where(m => m.InstructorId == accountId)
            .CountAsync(cancellationToken);

        // Get zoom meetings count
        var totalZoomMeetings = await _db.ZoomMeetings
            .Where(zm => zm.InstructorId == accountId)
            .CountAsync(cancellationToken);

        // Get upcoming zoom meetings
        var upcomingMeetings = await _db.ZoomMeetings
            .Include(zm => zm.CourseRound)
            .Include(zm => zm.CourseRound.Course)
            .Where(zm => zm.InstructorId == accountId && zm.MeetingDateTime > DateTime.UtcNow)
            .OrderBy(zm => zm.MeetingDateTime)
            .Take(10)
            .Select(zm => new
            {
                id = zm.Id,
                topic = zm.Topic,
                courseRoundId = zm.CourseRoundId,
                courseTitle = zm.CourseRound.Course.Title,
                meetingDateTime = zm.MeetingDateTime,
                meetingLink = zm.MeetingLink
            })
            .ToListAsync(cancellationToken);

        // Get recent course rounds
        var recentCourseRounds = await _db.CourseRounds
            .Include(cr => cr.Course)
            .Include(cr => cr.Status)
            .Where(cr => cr.MainInstructorId == accountId)
            .OrderByDescending(cr => cr.CreatedAt)
            .Take(10)
            .Select(cr => new
            {
                id = cr.Id,
                courseId = cr.CourseId,
                courseTitle = cr.Course.Title,
                roundNumber = cr.RoundNumber,
                startDate = cr.StartDate,
                endDate = cr.EndDate,
                statusId = cr.StatusId,
                statusName = cr.Status.StatusName,
                studentCount = cr.Students.Count
            })
            .ToListAsync(cancellationToken);

        // Get course rounds with student counts
        var courseRoundsWithStudents = await _db.CourseRounds
            .Include(cr => cr.Course)
            .Where(cr => cr.MainInstructorId == accountId)
            .Select(cr => new
            {
                id = cr.Id,
                courseTitle = cr.Course.Title,
                roundNumber = cr.RoundNumber,
                studentCount = cr.Students.Count
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            summary = new
            {
                myCourseRounds,
                totalStudents,
                activeCourseRounds,
                totalMaterials,
                totalZoomMeetings
            },
            upcomingMeetings,
            recentCourseRounds,
            courseRoundsWithStudents
        });
    }
}
