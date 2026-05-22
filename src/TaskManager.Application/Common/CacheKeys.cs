namespace TaskManager.Application.Common;

public static class CacheKeys
{
    public static string UserProjects(Guid userId) => $"user:{userId}:projects";
    public static string UserProject(Guid userId, Guid projectId) => $"user:{userId}:project:{projectId}";
    public static string ProjectTasks(Guid userId, Guid projectId) => $"user:{userId}:project:{projectId}:tasks";
}
