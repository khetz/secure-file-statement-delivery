using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SecureFileStatementAPI.Configuration;

namespace SecureFileStatementAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection BindConfig<T>(this IServiceCollection services, 
        IConfiguration configuration, string sectionName) where T : class
    {
        services.Configure<T>(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var dbSettings = serviceProvider.GetRequiredService<IOptions<DataBaseConfig>>().Value;
            options.UseSqlite(dbSettings.DefaultConnection);
        });

        return services;
    }
}
