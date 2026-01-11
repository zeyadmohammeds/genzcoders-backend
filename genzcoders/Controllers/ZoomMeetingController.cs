using genzcoders.DTOs.ZoomMeetingDto;
using genzcoders.Services.ZoomMeetingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ZoomMeetingController : ControllerBase
{
    private readonly IZoomMeetingService _service;

    public ZoomMeetingController(IZoomMeetingService service)
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
        var meetings = await _service.GetByCourseRoundIdAsync(courseRoundId);
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var meeting = await _service.GetByIdAsync(id);
        if (meeting == null) return NotFound();
        return Ok(meeting);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateZoomMeetingDto dto)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var meeting = await _service.CreateAsync(dto, instructorId);
            return CreatedAtAction(nameof(GetById), new { id = meeting.Id }, meeting);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateZoomMeetingDto dto)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var meeting = await _service.UpdateAsync(id, dto, instructorId);
            if (meeting == null) return NotFound();
            return Ok(meeting);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var deleted = await _service.DeleteAsync(id, instructorId);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
