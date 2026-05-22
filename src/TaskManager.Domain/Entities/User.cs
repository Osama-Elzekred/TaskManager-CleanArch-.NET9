namespace TaskManager.Domain.Entities;

using Common;
using Enums;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
