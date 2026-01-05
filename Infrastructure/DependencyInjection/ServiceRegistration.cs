using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();
        // Persistence
        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IProjectRepository,ProjectRepository>();
        services.AddScoped<ITaskRepository,TaskRepository>();
        services.AddScoped<IProjectStatusRepository,ProjectStatusRepository>();

        // Security
        services.Configure<JwtConfigurationModel>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtSettings, JwtSettings>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();

        // Storage

        return services;
    }
}
