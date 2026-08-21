namespace Domain.Entities;

public class Statement
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string StoragePath { get; set; }
    public required string PeriodCovered { get; set; }
    public long FileSize { get; set; }
    public required string ContentHash { get; set; }
    public DateTimeOffset UploadTimestamp { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
