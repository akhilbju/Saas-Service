public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthServices, AuthService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IProjectServices, ProjectServices>();

        return services;
    }
}