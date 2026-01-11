using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace genzcoders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HealthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "ok",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
    }

    [HttpGet("db")]
    public async Task<IActionResult> HealthDb(CancellationToken cancellationToken)
    {
        var connString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connString))
            return StatusCode(StatusCodes.Status500InternalServerError, "Missing connection string 'DefaultConnection'.");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await using var conn = new SqlConnection(connString);
            await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            cmd.CommandTimeout = 10;

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            sw.Stop();

            return Ok(new
            {
                status = "ok",
                db = "connected",
                elapsedMs = sw.ElapsedMilliseconds,
                result
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "error",
                db = "failed",
                elapsedMs = sw.ElapsedMilliseconds,
                message = ex.Message
            });
        }
    }
}
