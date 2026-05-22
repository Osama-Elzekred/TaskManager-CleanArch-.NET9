using FluentAssertions;
using Moq;
using TaskManager.Application.Admin.Queries;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tests.Admin;

public class AdminHandlersTests
{
    [Fact]
    public async Task GetUserCountQueryHandler_ReturnsUserCount()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Email = "a@test.com", Role = UserRole.User },
            new() { Id = Guid.NewGuid(), Email = "b@test.com", Role = UserRole.Admin }
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.Users.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var handler = new GetUserCountQueryHandler(unitOfWork.Object);
        var count = await handler.Handle(new GetUserCountQuery(), CancellationToken.None);

        count.Should().Be(2);
    }
}
