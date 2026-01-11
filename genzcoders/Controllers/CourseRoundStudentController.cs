using genzcoders.DTOs.CourseRoundStudentDto;
using genzcoders.Services.CourseRoundStudentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseRoundStudentController : ControllerBase
{
    private readonly ICourseRoundStudentService _service;

    public CourseRoundStudentController(ICourseRoundStudentService service)
    {
        _service = service;
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException("Invalid user ID");
    }

    [HttpGet("course-round/{courseRoundId}")]
    public async Task<IActionResult> GetByCourseRoundId(long courseRoundId)
    {
        var assignments = await _service.GetByCourseRoundIdAsync(courseRoundId);
        return Ok(assignments);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetByStudentId(long studentId)
    {
        var assignments = await _service.GetByStudentIdAsync(studentId);
        return Ok(assignments);
    }

    [HttpPost("course-round/{courseRoundId}/assign")]
    public async Task<IActionResult> AssignStudent(long courseRoundId, [FromBody] AssignStudentDto dto)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var assignment = await _service.AssignStudentAsync(courseRoundId, dto, instructorId);
            return Ok(assignment);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> UnassignStudent(long id)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var deleted = await _service.UnassignStudentAsync(id, instructorId);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
