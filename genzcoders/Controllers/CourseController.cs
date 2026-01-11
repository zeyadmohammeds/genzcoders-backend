using genzcoders.DTOs.CoursesDto;
using genzcoders.Services.CourseService;
using Microsoft.AspNetCore.Mvc;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly ICourseService _service;

    public CourseController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableCourses()
    {
        var courses = await _service.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetailedCourse(string id)
    {
        if (!long.TryParse(id, out var courseId))
            return BadRequest("Invalid course ID");

        var course = await _service.GetCourseByIdAsync(courseId);
        if (course == null) return NotFound("Course not found");

        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var course = await _service.CreateCourseAsync(dto);
        return Ok(course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCourse(string id, [FromBody] UpdateCourseDto dto)
    {
        if (!long.TryParse(id, out var courseId))
            return BadRequest("Invalid course ID");

        var updated = await _service.UpdateCourseAsync(courseId, dto);
        if (updated == null) return NotFound("Course not Found");

        return Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchCourse(string id, [FromBody] PatchCourseDto dto)
    {
        if (!long.TryParse(id, out var courseId))
            return BadRequest("Invalid course ID");

        var updated = await _service.PatchCourseAsync(courseId, dto);
        if (!updated) return NotFound("Course not found");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(string id)
    {
        if (!long.TryParse(id, out var courseId))
            return BadRequest("Invalid course ID");

        var deleted = await _service.DeleteCourseAsync(courseId);
        if (!deleted) return NotFound("Course Not found");

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCourses([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search name is required.");

        var result = await _service.SearchCoursesByNameAsync(name);
        return Ok(result);
    }

    [HttpGet("instructor/{instructorId}")]
    public async Task<IActionResult> GetCoursesByInstructor(long instructorId)
    {
        var courses = await _service.GetCoursesByInstructorAsync(instructorId);
        return Ok(courses);
    }
}
