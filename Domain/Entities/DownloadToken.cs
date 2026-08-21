namespace Domain.Entities;

public class DownloadToken
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid StatementId { get; set; }
    public required string TokenString { get; set; }
    public DateTimeOffset CreationTime { get; set; }
    public DateTimeOffset ExpirationTime { get; set; }
    public bool Used { get; set; }
    public DateTimeOffset UsedAt { get; set; }
    public string? UsedByIp { get; set; }
}
