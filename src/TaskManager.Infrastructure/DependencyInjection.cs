using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Infrastructure.Caching;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Data.Interceptors;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Register the soft delete interceptor as a singleton (stateless, thread-safe)
        services.AddSingleton<AuditableInterceptor>();

        services.AddDbContextPool<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, b =>
                {
                    b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure();
                })
                .AddInterceptors(sp.GetRequiredService<AuditableInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICacheService, DistributedCacheService>();

        // Metrics service for simple in-memory counters
        services.AddSingleton<MetricsService>();

        // Configure cache options (TTL)
        services.Configure<CacheOptions>(options =>
            configuration.GetSection("CacheOptions").Bind(options));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings configuration not found.");
        services.AddSingleton(jwtSettings);
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}
