using genzcoders.DTOs.WeekDto;
using genzcoders.Services.WeekService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WeekController : ControllerBase
{
    private readonly IWeekService _service;

    public WeekController(IWeekService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetForGenzcoders()
    {
        var result = await _service.GetByBusinessEntityNameAsync("genzcoders");
        return Ok(result);
    }

    [HttpGet("course-round/{courseRoundId:long}")]
    public async Task<IActionResult> GetByCourseRoundId(long courseRoundId)
    {
        var result = await _service.GetByCourseRoundIdAsync(courseRoundId, "genzcoders");
        return Ok(result);
    }

    [HttpGet("course-round/{courseRoundId:long}/business/{businessEntityName}")]
    public async Task<IActionResult> GetByCourseRoundIdAndBusinessEntity(long courseRoundId, string businessEntityName)
    {
        var result = await _service.GetByCourseRoundIdAsync(courseRoundId, businessEntityName);
        return Ok(result);
    }

    [HttpGet("business/{businessEntityName}")]
    public async Task<IActionResult> GetByBusinessEntityName(string businessEntityName)
    {
        var result = await _service.GetByBusinessEntityNameAsync(businessEntityName);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWeekDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateWeekDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
