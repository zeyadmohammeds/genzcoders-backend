using genzcoders.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("instructors")]
    public async Task<IActionResult> GetInstructors()
    {
        var instructors = await _context.Accounts
            .Where(a => a.RoleId == 2 && a.IsActive)
            .Select(a => new
            {
                id = a.Id,
                fullNameEn = a.FullNameEn,
                fullNameAr = a.FullNameAr,
                email = a.Email,
                phone = a.Phone
            })
            .ToListAsync();

        return Ok(instructors);
    }
}
