namespace TaskManager.Domain.Entities;

using Common;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    public User? User { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
