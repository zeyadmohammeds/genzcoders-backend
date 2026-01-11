using genzcoders.DTOs.TestItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace genzcoders.Controllers;

[ApiController]
[Route("api/test-items")]
public class TestItemsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TestItemsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<List<TestItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var connString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
            return StatusCode(StatusCodes.Status500InternalServerError, "Missing connection string 'DefaultConnection'.");

        var results = new List<TestItemDto>();

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT Id, Name, Notes, CreatedAtUtc
FROM dbo.TestItems
ORDER BY Id DESC";
        cmd.CommandTimeout = 10;

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new TestItemDto
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Notes = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedAtUtc = reader.GetDateTime(3)
            });
        }

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TestItemDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var connString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
            return StatusCode(StatusCodes.Status500InternalServerError, "Missing connection string 'DefaultConnection'.");

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT Id, Name, Notes, CreatedAtUtc
FROM dbo.TestItems
WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = id });
        cmd.CommandTimeout = 10;

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return NotFound();

        var dto = new TestItemDto
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Notes = reader.IsDBNull(2) ? null : reader.GetString(2),
            CreatedAtUtc = reader.GetDateTime(3)
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TestItemDto>> Create([FromBody] CreateTestItemRequestDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var connString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
            return StatusCode(StatusCodes.Status500InternalServerError, "Missing connection string 'DefaultConnection'.");

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.TestItems (Name, Notes)
OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.Notes, INSERTED.CreatedAtUtc
VALUES (@Name, @Notes);";
        cmd.Parameters.Add(new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = dto.Name.Trim() });
        cmd.Parameters.Add(new SqlParameter("@Notes", System.Data.SqlDbType.NVarChar, -1)
        {
            Value = string.IsNullOrWhiteSpace(dto.Notes) ? DBNull.Value : dto.Notes.Trim()
        });
        cmd.CommandTimeout = 10;

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        var created = new TestItemDto
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Notes = reader.IsDBNull(2) ? null : reader.GetString(2),
            CreatedAtUtc = reader.GetDateTime(3)
        };

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTestItemRequestDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var connString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
            return StatusCode(StatusCodes.Status500InternalServerError, "Missing connection string 'DefaultConnection'.");

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
UPDATE dbo.TestItems
SET Name = @Name,
    Notes = @Notes
WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = id });
        cmd.Parameters.Add(new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = dto.Name.Trim() });
        cmd.Parameters.Add(new SqlParameter("@Notes", System.Data.SqlDbType.NVarChar, -1)
        {
            Value = string.IsNullOrWhiteSpace(dto.Notes) ? DBNull.Value : dto.Notes.Trim()
        });
        cmd.CommandTimeout = 10;

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rows == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var connString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
            return StatusCode(StatusCodes.Status500InternalServerError, "Missing connection string 'DefaultConnection'.");

        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.TestItems WHERE Id = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = id });
        cmd.CommandTimeout = 10;

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rows == 0)
            return NotFound();

        return NoContent();
    }
}
