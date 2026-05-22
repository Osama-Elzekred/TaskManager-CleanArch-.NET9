using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasMany(u => u.Projects)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
