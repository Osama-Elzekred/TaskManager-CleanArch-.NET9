using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManager.Domain.Common;

namespace TaskManager.Infrastructure.Data.Interceptors;

public class AuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not null)
        {
            ApplyAuditing(context);
            ApplySoftDeletes(context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyAuditing(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
    private static void ApplySoftDeletes(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted) continue;

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;

            // Cascade soft delete to loaded child entities
            CascadeSoftDelete(context, entry.Entity);
        }
    }

    /// <summary>
    /// Recursively soft-deletes loaded child entities when parent is soft-deleted.
    /// Only affects entities in the ChangeTracker - use ExecuteUpdateAsync for bulk operations.
    /// </summary>
    private static void CascadeSoftDelete(DbContext context, ISoftDeletable parentEntity)
    {
        var parentEntry = context.Entry(parentEntity);

        foreach (var navigation in parentEntry.Navigations)
        {
            if (navigation.CurrentValue is null) continue;

            if (navigation.CurrentValue is IEnumerable<ISoftDeletable> children)
            {
                foreach (var child in children)
                {
                    if (child.IsDeleted) continue;
                    child.IsDeleted = true;
                    child.DeletedAt = DateTime.UtcNow;
                    child.DeletedBy = parentEntity.DeletedBy;
                    context.Entry(child).State = EntityState.Modified;
                }
            }
            else if (navigation.CurrentValue is ISoftDeletable child && !child.IsDeleted)
            {
                child.IsDeleted = true;
                child.DeletedAt = DateTime.UtcNow;
                child.DeletedBy = parentEntity.DeletedBy;
                context.Entry(child).State = EntityState.Modified;
            }
        }
    }
}
