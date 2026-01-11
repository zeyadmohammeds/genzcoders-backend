using genzcoders.Data;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Helpers;

public static class RoleHelper
{
    public const string Admin = "Admin";
    public const string Instructor = "Instructor";
    public const string Student = "Student";

    public static async Task<long?> GetRoleIdByNameAsync(AppDbContext db, string roleName, CancellationToken cancellationToken = default)
    {
        var role = await db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower(), cancellationToken);

        return role?.Id;
    }
}
