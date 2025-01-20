namespace Sun.Models;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

