using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder AddSeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                FullName = "Vukheta Maluleke",
                Email = "vukheta@gmail.com",
                PasswordHash = ComputeHash("Password123")
            },
            new Customer
            {
                FullName = "Seeded User",
                Email = "seeded@gmail.com",
                PasswordHash = ComputeHash("Password456")
            });

        return modelBuilder;
    }

    private static string ComputeHash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
