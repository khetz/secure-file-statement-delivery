using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder AddCustomerSeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                FullName = "Vukheta Maluleke",
                Email = "vukheta@gmail.com",
                PasswordHash = ComputeHash("Password123"),
                AccountNumber = 100,
                Active = true
            },
            new Customer
            {
                Id = Guid.Parse("f9e8d7c6-b5a4-3f2e-1d0c-9b8a7f6e5d4c"),
                FullName = "Seeded User",
                Email = "seeded@gmail.com",
                PasswordHash = ComputeHash("Password456"),
                AccountNumber = 150,
                Active = true
            });

        return modelBuilder;
    }

    private static string ComputeHash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
