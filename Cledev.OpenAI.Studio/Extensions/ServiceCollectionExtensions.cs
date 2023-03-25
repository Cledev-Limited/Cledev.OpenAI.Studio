namespace Cledev.OpenAI.Studio.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStudioSettings(this IServiceCollection services)
    {
        {
            var requiredService = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.Configure<StudioSettings>(requiredService.GetSection("Studio"));
            services.AddOptions<StudioSettings>();
            return services;
        }
    }
}
