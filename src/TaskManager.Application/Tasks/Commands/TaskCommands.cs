namespace TaskManager.Application.Tasks.Commands;

using Dtos;
using MediatR;

public class CreateTaskCommand : IRequest<TaskDto>
{
  public Guid ProjectId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
  public DateTime? DueDate { get; set; }
  public Priority Priority { get; set; }
}

public class UpdateTaskCommand : IRequest<TaskDto>
{
  public Guid TaskId { get; set; }
  public Guid ProjectId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public TaskStatus Status { get; set; }
  public DateTime? DueDate { get; set; }
  public Priority Priority { get; set; }
}

public class DeleteTaskCommand : IRequest<Unit>
{
  public Guid TaskId { get; set; }
  public Guid ProjectId { get; set; }
}
