using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Common;

namespace TaskManager.Infrastructure.Data.Extensions;

/// <summary>
/// Manages soft-delete filter and index configuration globally.
/// Combines filter + index to prevent full table scans on soft-deleted entities.
/// </summary>
public static class QueryFilterExtensions
{
  private const string SoftDeleteFilterName = "SoftDelete";

  /// <summary>
  /// Applies soft-delete filter and IsDeleted index.
  /// TODO (EF Core 10+): Change to HasQueryFilter(SoftDeleteFilterName, e => !e.IsDeleted);
  /// </summary>
  public static void ConfigureSoftDelete<T>(this EntityTypeBuilder<T> builder)
      where T : class, ISoftDeletable
  {
    builder.HasQueryFilter(e => !e.IsDeleted);
    builder.HasIndex(e => e.IsDeleted)
        .HasDatabaseName($"IX_{typeof(T).Name}_IsDeleted");
  }

  /// <summary>
  /// Applies soft-delete configuration to all ISoftDeletable entities.
  /// Single call handles current and future soft-deletable entities automatically.
  /// </summary>
  public static void ConfigureGlobalSoftDeletes(this ModelBuilder modelBuilder)
  {
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
        continue;

      // Use reflection to invoke the generic method with the correct type
      var entityClrType = entityType.ClrType;
      var method = typeof(QueryFilterExtensions)
          .GetMethod(nameof(ConfigureSoftDelete),
              System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)?
          .MakeGenericMethod(entityClrType);

      if (method != null)
      {
        var builder = modelBuilder.Entity(entityClrType);
        method.Invoke(null, new object[] { builder });
      }
    }
  }
}
