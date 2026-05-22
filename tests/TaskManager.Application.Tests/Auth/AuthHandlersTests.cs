using FluentAssertions;
using Moq;
using TaskManager.Application.Auth.Commands;
using TaskManager.Application.Auth.Handlers;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Auth;

public class AuthHandlersTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;

    public AuthHandlersTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
    }

    [Fact]
    public async Task RegisterCommandHandler_WithValidInput_CreatesUserAndReturnsAuthResponse()
    {
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePassword123!"
        };

        _mockUnitOfWork.Setup(x => x.Users.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        _mockPasswordHasher.Setup(x => x.HashPassword(command.Password))
            .Returns("hashed_password");

        _mockJwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<Guid>(), command.Email, It.IsAny<string>()))
            .Returns("valid_token");

        var handler = new RegisterCommandHandler(_mockUnitOfWork.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email);
        result.FullName.Should().Be(command.FullName);
        result.Token.Should().Be("valid_token");
        _mockUnitOfWork.Verify(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterCommandHandler_WithExistingEmail_ThrowsInvalidOperationException()
    {
        var existingUser = new User { Id = Guid.NewGuid(), Email = "john@example.com" };
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePassword123!"
        };

        _mockUnitOfWork.Setup(x => x.Users.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { existingUser });

        var handler = new RegisterCommandHandler(_mockUnitOfWork.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task LoginCommandHandler_WithValidCredentials_ReturnsAuthResponse()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "john@example.com",
            FullName = "John Doe",
            PasswordHash = "hashed_password"
        };

        var command = new LoginCommand
        {
            Email = "john@example.com",
            Password = "SecurePassword123!"
        };

        _mockUnitOfWork.Setup(x => x.Users.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);

        _mockJwtTokenGenerator.Setup(x => x.GenerateToken(user.Id, user.Email, It.IsAny<string>()))
            .Returns("valid_token");

        var handler = new LoginCommandHandler(_mockUnitOfWork.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Token.Should().Be("valid_token");
    }

    [Fact]
    public async Task LoginCommandHandler_WithInvalidPassword_ThrowsNotFoundException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "john@example.com",
            FullName = "John Doe",
            PasswordHash = "hashed_password"
        };

        var command = new LoginCommand
        {
            Email = "john@example.com",
            Password = "WrongPassword"
        };

        _mockUnitOfWork.Setup(x => x.Users.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(false);

        var handler = new LoginCommandHandler(_mockUnitOfWork.Object, _mockPasswordHasher.Object, _mockJwtTokenGenerator.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
