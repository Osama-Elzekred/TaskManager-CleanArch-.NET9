namespace TaskManager.Application.Tasks.Dtos;

using Domain.Enums;

public class CreateTaskRequest
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
  public DateTime? DueDate { get; set; }
  public Priority Priority { get; set; }
}

public class UpdateTaskRequest
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
  public DateTime? DueDate { get; set; }
  public Priority Priority { get; set; }
}

public class UpdateTaskStatusRequest
{
  public TaskStatus Status { get; set; }
}

public class TaskDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
  public DateTime? DueDate { get; set; }
  public Priority Priority { get; set; }
  public Guid ProjectId { get; set; }
}

public enum TaskStatus
{
  Todo = 0,
  InProgress = 1,
  Done = 2
}

public enum Priority
{
  Low = 0,
  Medium = 1,
  High = 2
}
