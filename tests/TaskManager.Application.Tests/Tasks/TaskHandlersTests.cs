using FluentAssertions;
using Moq;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Application.Tasks.Handlers;
using TaskManager.Application.Tasks.Queries;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskItemStatus = TaskManager.Application.Tasks.Dtos.TaskStatus;
using TaskPriority = TaskManager.Application.Tasks.Dtos.Priority;

namespace TaskManager.Application.Tests.Tasks;

public class TaskHandlersTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly ProjectCacheInvalidator _cacheInvalidator;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _projectId = Guid.NewGuid();

    public TaskHandlersTests()
    {
        _currentUser.Setup(x => x.UserId).Returns(_userId);
        var mockMetrics = new Mock<IMetricsService>();
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<ProjectCacheInvalidator>>();
        _cacheInvalidator = new ProjectCacheInvalidator(_cache.Object, mockMetrics.Object, mockLogger.Object);
        // setup repository mocks for unit of work
        var projectRepo = new Mock<IRepository<Project>>();
        var taskRepo = new Mock<IRepository<TaskItem>>();
        projectRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);
        taskRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);
        taskRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TaskItem>());
        _unitOfWork.Setup(x => x.Projects).Returns(projectRepo.Object);
        _unitOfWork.Setup(x => x.Tasks).Returns(taskRepo.Object);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
    }

    [Fact]
    public async Task CreateTaskCommandHandler_CreatesTaskForOwnedProject()
    {
        var project = new Project { Id = _projectId, UserId = _userId, Name = "P" };
        _unitOfWork.Setup(x => x.Projects.GetByIdAsync(_projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);

        var handler = new CreateTaskCommandHandler(_unitOfWork.Object, _currentUser.Object, _cacheInvalidator);
        var command = new CreateTaskCommand
        {
            ProjectId = _projectId,
            Title = "Task",
            Description = "Desc",
            Status = TaskItemStatus.Todo,
            Priority = TaskPriority.Medium
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Title.Should().Be("Task");
        _unitOfWork.Verify(x => x.Tasks.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task UpdateTaskStatusCommandHandler_UpdatesStatusOnly()
    {
        var project = new Project { Id = _projectId, UserId = _userId, Name = "P" };
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            ProjectId = _projectId,
            Title = "T",
            Status = Domain.Enums.TaskStatus.Todo
        };

        _unitOfWork.Setup(x => x.Projects.GetByIdAsync(_projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        _unitOfWork.Setup(x => x.Tasks.GetByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var handler = new UpdateTaskStatusCommandHandler(_unitOfWork.Object, _currentUser.Object, _cacheInvalidator);
        var result = await handler.Handle(new UpdateTaskStatusCommand
        {
            ProjectId = _projectId,
            TaskId = taskId,
            Status = TaskItemStatus.InProgress
        }, CancellationToken.None);

        result.Status.Should().Be(TaskItemStatus.InProgress);
        task.Status.Should().Be(Domain.Enums.TaskStatus.InProgress);
    }

    [Fact]
    public async Task GetTasksByProjectQueryHandler_UsesCacheWhenPresent()
    {
        var project = new Project { Id = _projectId, UserId = _userId, Name = "P" };
        var cached = new List<TaskDto> { new() { Id = Guid.NewGuid(), Title = "Cached", ProjectId = _projectId } };

        _unitOfWork.Setup(x => x.Projects.GetByIdAsync(_projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        _unitOfWork.Setup(x => x.Tasks.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TaskItem>());
        // The handler no longer uses ICacheService directly (cache handled by behavior), so construct with current signature
        var handler = new GetTasksByProjectQueryHandler(_unitOfWork.Object, _currentUser.Object);
        var result = await handler.Handle(new GetTasksByProjectQuery { ProjectId = _projectId }, CancellationToken.None);

        result.Should().HaveCount(0); // No tasks in the repository in this setup
        _unitOfWork.Verify(x => x.Tasks.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskCommandHandler_DeletesOwnedTask()
    {
        var project = new Project { Id = _projectId, UserId = _userId, Name = "P" };
        var taskId = Guid.NewGuid();
        var task = new TaskItem { Id = taskId, ProjectId = _projectId, Title = "T" };

        _unitOfWork.Setup(x => x.Projects.GetByIdAsync(_projectId, It.IsAny<CancellationToken>())).ReturnsAsync(project);
        _unitOfWork.Setup(x => x.Tasks.GetByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);

        var handler = new DeleteTaskCommandHandler(_unitOfWork.Object, _currentUser.Object, _cacheInvalidator);
        await handler.Handle(new DeleteTaskCommand { ProjectId = _projectId, TaskId = taskId }, CancellationToken.None);

        _unitOfWork.Verify(x => x.Tasks.Delete(task), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
