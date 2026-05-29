namespace TaskManager.Domain.Common;

/// <summary>
/// Marker interface for entities supporting soft delete.
/// Soft-deleted entities are marked as deleted instead of physically removed,
/// enabling data recovery, audit compliance, and referential integrity preservation.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
