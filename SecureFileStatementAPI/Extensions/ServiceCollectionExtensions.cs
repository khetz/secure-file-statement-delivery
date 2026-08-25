using Application.Configuration;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtAuthenticationConfig();
        configuration.GetSection(JwtAuthenticationConfig.JwtAuthenticationSectionName).Bind(jwtSettings);

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = JwtHelper.GetSigningKey(jwtSettings.Secret)
                };
            });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}