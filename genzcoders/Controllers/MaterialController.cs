using genzcoders.DTOs.MaterialDto;
using genzcoders.Services.MaterialService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService _service;

    public MaterialController(IMaterialService service)
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
        var materials = await _service.GetByCourseRoundIdAsync(courseRoundId);
        return Ok(materials);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var material = await _service.GetByIdAsync(id);
        if (material == null) return NotFound();
        return Ok(material);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMaterialDto dto)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var material = await _service.CreateAsync(dto, instructorId);
            return CreatedAtAction(nameof(GetById), new { id = material.Id }, material);
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
    public async Task<IActionResult> Update(long id, [FromBody] UpdateMaterialDto dto)
    {
        try
        {
            var instructorId = GetCurrentUserId();
            var material = await _service.UpdateAsync(id, dto, instructorId);
            if (material == null) return NotFound();
            return Ok(material);
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
