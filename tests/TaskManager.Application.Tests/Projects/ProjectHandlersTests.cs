using FluentAssertions;
using Moq;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Projects.Commands;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Application.Projects.Handlers;
using TaskManager.Application.Projects.Queries;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Projects;

public class ProjectHandlersTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICurrentUserService> _mockCurrentUserService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ProjectCacheInvalidator _cacheInvalidator;
    private readonly Guid _userId = Guid.NewGuid();

    public ProjectHandlersTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCurrentUserService = new Mock<ICurrentUserService>();
        _mockCacheService = new Mock<ICacheService>();
        _mockCurrentUserService.Setup(x => x.UserId).Returns(_userId);
        // setup repository mocks for unit of work
        var projectRepo = new Mock<IRepository<Project>>();
        projectRepo.Setup(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        projectRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Project>());
        _mockUnitOfWork.Setup(x => x.Projects).Returns(projectRepo.Object);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var mockMetrics = new Mock<IMetricsService>();
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ProjectCacheInvalidator>>();
        _cacheInvalidator = new ProjectCacheInvalidator(_mockCacheService.Object, mockMetrics.Object, mockLogger.Object);
    }

    [Fact]
    public async Task CreateProjectCommandHandler_WithValidInput_CreatesAndReturnsProject()
    {
        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var handler = new CreateProjectCommandHandler(
            _mockUnitOfWork.Object, _mockCurrentUserService.Object, _cacheInvalidator);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.UserId.Should().Be(_userId);
        _mockUnitOfWork.Verify(x => x.Projects.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllProjectsQueryHandler_ReturnsOnlyUserProjects()
    {
        var userProject = new Project { Id = Guid.NewGuid(), Name = "User Project", UserId = _userId, IsDeleted = false };
        var otherUserProject = new Project { Id = Guid.NewGuid(), Name = "Other Project", UserId = Guid.NewGuid() };

        _mockUnitOfWork.Setup(x => x.Projects.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project> { userProject, otherUserProject });

        _mockCacheService.Setup(x => x.GetAsync<List<ProjectDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<ProjectDto>?)null);

        var handler = new GetAllProjectsQueryHandler(
            _mockUnitOfWork.Object, _mockCurrentUserService.Object);

        var result = await handler.Handle(new GetAllProjectsQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(userProject.Id);
    }
}
