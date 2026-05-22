using FluentAssertions;
using TaskManager.Application.Auth.Commands;
using TaskManager.Application.Auth.Validators;
using TaskManager.Application.Projects.Commands;
using TaskManager.Application.Projects.Validators;

namespace TaskManager.Application.Tests.Validators;

public class ValidatorTests
{
    [Fact]
    public void RegisterCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new RegisterCommandValidator();
        var request = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePassword123!"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegisterCommandValidator_WithInvalidEmail_FailsValidation()
    {
        var validator = new RegisterCommandValidator();
        var request = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "invalid-email",
            Password = "SecurePassword123!"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void CreateProjectCommandValidator_WithEmptyName_FailsValidation()
    {
        var validator = new CreateProjectCommandValidator();
        var request = new CreateProjectCommand
        {
            Name = "",
            Description = "Description"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void CreateProjectCommandValidator_WithValidData_PassesValidation()
    {
        var validator = new CreateProjectCommandValidator();
        var request = new CreateProjectCommand
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}
