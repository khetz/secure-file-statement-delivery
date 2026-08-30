using Infrastructure.Contexts;
using Infrastructure.Data;

namespace SecureStatementDelivery.Api.Extensions;

public static class SeederExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await context.Database.EnsureCreatedAsync();

        var seeder = new DatabaseSeeder(context, configuration);
        await seeder.SeedAsync();
    }
}