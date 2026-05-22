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
        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>().Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;
        }
    }
}
