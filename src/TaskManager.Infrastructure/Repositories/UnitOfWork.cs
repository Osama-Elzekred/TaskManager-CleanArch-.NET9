using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<User>? _users;
    private IRepository<Project>? _projects;
    private IRepository<TaskItem>? _tasks;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Project> Projects => _projects ??= new Repository<Project>(_context);
    public IRepository<TaskItem> Tasks => _tasks ??= new Repository<TaskItem>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
