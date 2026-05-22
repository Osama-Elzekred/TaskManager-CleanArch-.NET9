namespace TaskManager.Domain.Entities;

using Common;
using Enums;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public Guid ProjectId { get; set; }

    public Project? Project { get; set; }
}
