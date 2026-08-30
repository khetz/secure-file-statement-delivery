using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public DatabaseSeeder(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        if (_context.Customers.Any()) return;

        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "john.doe@example.com",
            FullName = "John Doe",
            AccountNumber = 12,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Active = true
        };

        var customer2 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "jane.smith@example.com",
            FullName = "Jane Smith",
            AccountNumber = 13,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password456!"),
            Active = true
        };

        _context.Customers.AddRange(customer1, customer2);
        await _context.SaveChangesAsync();

        var basePath = _configuration["Storage:BasePath"] ?? "Statements";
        Directory.CreateDirectory(basePath);

        var statements = new List<(Customer customer, string period, string fileName)>
        {
            (customer1, "2026-01", "January_2026_Statement.pdf"),
            (customer1, "2026-02", "February_2026_Statement.pdf"),
            (customer1, "2026-03", "March_2026_Statement.pdf"),
            (customer2, "2026-01", "January_2026_Statement.pdf"),
            (customer2, "2026-02", "February_2026_Statement.pdf"),
        };

        foreach (var (customer, period, fileName) in statements)
        {
            var pdfBytes = GenerateSamplePdf(customer.FullName, customer.AccountNumber, period);

            var storageName = $"{Guid.NewGuid()}_{fileName}";
            var fullPath = Path.Combine(basePath, storageName);
            await File.WriteAllBytesAsync(fullPath, pdfBytes);

            var hash = Convert.ToHexString(SHA256.HashData(pdfBytes));

            var statement = new Statement
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                FileName = fileName,
                StoragePath = storageName,
                PeriodCovered = period,
                FileSize = pdfBytes.Length,
                ContentHash = hash,
                UploadTimestamp = DateTimeOffset.UtcNow
            };

            _context.Statement.Add(statement);
        }

        await _context.SaveChangesAsync();

        Console.WriteLine("=== Database Seeded ===");
        Console.WriteLine("  Customer 1: john.doe@example.com / Password123!");
        Console.WriteLine("  Customer 2: jane.smith@example.com / Password456!");
        Console.WriteLine($"  Statements: {statements.Count} sample PDFs created");
        Console.WriteLine("=======================");
    }

    private byte[] GenerateSamplePdf(string customerName, int accountNumber, string period)
    {
        var text = $"BT /F1 16 Tf 50 700 Td (Account Statement) Tj ET " +
                   $"BT /F1 12 Tf 50 660 Td (Customer: {customerName}) Tj ET " +
                   $"BT /F1 12 Tf 50 640 Td (Account: {accountNumber}) Tj ET " +
                   $"BT /F1 12 Tf 50 620 Td (Period: {period}) Tj ET " +
                   $"BT /F1 12 Tf 50 580 Td (Opening Balance: R 15,420.00) Tj ET " +
                   $"BT /F1 12 Tf 50 560 Td (Deposits: R 8,500.00) Tj ET " +
                   $"BT /F1 12 Tf 50 540 Td (Withdrawals: R 6,230.00) Tj ET " +
                   $"BT /F1 12 Tf 50 520 Td (Closing Balance: R 17,690.00) Tj ET " +
                   $"BT /F1 10 Tf 50 460 Td (This is a sample statement for testing purposes.) Tj ET";

        var content = new StringBuilder();
        content.AppendLine("%PDF-1.4");
        content.AppendLine("1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj");
        content.AppendLine("2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj");
        content.AppendLine("3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj");
        content.AppendLine($"4 0 obj << /Length {text.Length} >> stream");
        content.Append(text);
        content.AppendLine();
        content.AppendLine("endstream endobj");
        content.AppendLine("5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj");
        content.AppendLine("xref");
        content.AppendLine("0 6");
        content.AppendLine("0000000000 65535 f ");
        content.AppendLine("trailer << /Size 6 /Root 1 0 R >>");
        content.AppendLine("startxref");
        content.AppendLine("0");
        content.AppendLine("%%EOF");

        return Encoding.ASCII.GetBytes(content.ToString());
    }
}