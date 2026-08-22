using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<DownloadToken> DownloadTokens {  get; set; }
    public DbSet<Statement> Statement { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Statement>()
            .HasIndex(s => s.CustomerId);

        modelBuilder.Entity<DownloadToken>()
            .HasIndex(dt => dt.Token)
            .IsUnique();

        modelBuilder.Entity<DownloadToken>()
            .HasIndex(dt => dt.ExpirationTime);

        modelBuilder.AddCustomerSeedData();
    }
}
