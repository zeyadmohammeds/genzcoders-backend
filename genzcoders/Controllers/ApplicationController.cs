using genzcoders.DTOs.ApplicationDto;
using genzcoders.Services.ApplicationService;
using Microsoft.AspNetCore.Mvc;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _service;

    public ApplicationController(IApplicationService service)
    {
        _service = service;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API is running");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationDto dto)
    {
        var application = await _service.CreateAsync(dto);
        return Ok(application);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] PatchApplicationStatusDto dto)
    {
        if (!long.TryParse(id, out var applicationId))
            return BadRequest("Invalid application ID");

        var updated = await _service.UpdateStatusAsync(applicationId, dto);
        if (updated == null)
            return NotFound("Application not found");

        return Ok(updated);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!long.TryParse(id, out var applicationId))
            return BadRequest("Invalid application ID");

        var result = await _service.GetByIdAsync(applicationId);
        if (result == null) return NotFound("Application Not Found");

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateApplicationDto dto)
    {
        if (!long.TryParse(id, out var applicationId))
            return BadRequest("Invalid application ID");

        await _service.UpdateApplicationAsync(applicationId, dto);
        return Ok();
    }
}
