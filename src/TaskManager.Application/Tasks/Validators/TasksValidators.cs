namespace TaskManager.Application.Tasks.Validators;

using TaskManager.Application.Tasks.Commands;
using FluentValidation;

public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
  public UpdateTaskStatusCommandValidator()
  {
    RuleFor(x => x.TaskId).NotEmpty();
    RuleFor(x => x.ProjectId).NotEmpty();
    RuleFor(x => x.Status).IsInEnum();
  }
}
