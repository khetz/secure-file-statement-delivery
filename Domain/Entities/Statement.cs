namespace Domain.Entities;

public class Statement
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public required string FileName { get; set; }
    public required string StoragePath { get; set; }
    public required string PeriodCovered { get; set; }
    public int FileSize { get; set; }
    public required string ContentHash { get; set; }
    public DateTimeOffset UploadTimestamp { get; set; }
}
