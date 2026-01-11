namespace genzcoders.Models;

public class Role
{
    public long Id { get; set; }
    public string RoleName { get; set; } = null!;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
