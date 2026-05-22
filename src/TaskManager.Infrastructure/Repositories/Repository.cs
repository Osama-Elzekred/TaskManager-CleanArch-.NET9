using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Common;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => DbSet.Update(entity);

    public void Delete(T entity) => DbSet.Remove(entity);
}
