using Application.Configuration;
using Application.Helpers;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Configuration;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Storage;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SecureFileStatementAPI.Configuration;
using System.Net;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace SecureFileStatementAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection BindConfig<T>(this IServiceCollection services, 
        IConfiguration configuration, string sectionName) where T : class
    {
        services.Configure<T>(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection BindAllConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.BindConfig<DataBaseConfig>(configuration, DataBaseConfig.DatabaseSectionName);
        services.BindConfig<FileStorageConfig>(configuration, FileStorageConfig.StorageSectionName);
        services.BindConfig<JwtAuthenticationConfig>(configuration, JwtAuthenticationConfig.JwtAuthenticationSectionName);
        services.BindConfig<DownloadTokenConfig>(configuration, DownloadTokenConfig.DowloadTokenSectionName);

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
        services.AddScoped<IStatementRepository, StatementRepository>();
        services.AddScoped<IDownloadTokenRepository, DownloadTokenRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IStatementService, StatementService>();
        services.AddScoped<IDownloadTokenService, DownloadTokenService>();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(document => new()
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
                
        });

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddLinkGenerationPolicy();
            options.AddDownloadPolicy();
        });
    }

    private static void AddLinkGenerationPolicy(this RateLimiterOptions options)
    {
        options.AddPolicy("link-generation-policy", httpContext =>
        {
            string customerId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: customerId,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,                       
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
        });
    }

    private static void AddDownloadPolicy(this RateLimiterOptions options)
    {
        options.AddPolicy("download-policy", httpContext =>
        {
            string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
                          ?? IPAddress.Loopback.ToString();

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ipAddress,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
        });
    }
}