using FluentAssertions;
using TaskManager.Application.Auth.Commands;
using TaskManager.Application.Auth.Validators;
using TaskManager.Application.Projects.Commands;
using TaskManager.Application.Projects.Validators;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks.Validators;
using TaskStatus = TaskManager.Application.Tasks.Dtos.TaskStatus;

namespace TaskManager.Application.Tests.Validators;

public class CommandValidatorTests
{
    [Fact]
    public void RegisterCommandValidator_RejectsShortPassword()
    {
        var validator = new RegisterCommandValidator();
        var result = validator.Validate(new RegisterCommand
        {
            FullName = "John",
            Email = "john@test.com",
            Password = "123"
        });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateProjectCommandValidator_RejectsEmptyName()
    {
        var validator = new CreateProjectCommandValidator();
        var result = validator.Validate(new CreateProjectCommand { Name = "", Description = "x" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateTaskStatusCommandValidator_AcceptsValidCommand()
    {
        var validator = new UpdateTaskStatusCommandValidator();
        var result = validator.Validate(new UpdateTaskStatusCommand
        {
            ProjectId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Status = TaskStatus.Done
        });

        result.IsValid.Should().BeTrue();
    }
}
