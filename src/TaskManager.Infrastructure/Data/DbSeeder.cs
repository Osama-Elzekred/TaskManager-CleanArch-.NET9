using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Data;

public static class DbSeeder
{
    private const string AdminEmail = "admin@taskmanager.local";

    public static async Task SeedAdminAsync(AppDbContext db, IPasswordHasher passwordHasher)
    {
        if (await db.Users.AnyAsync(u => u.Email == AdminEmail))
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "System Admin",
            Email = AdminEmail,
            PasswordHash = passwordHasher.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
