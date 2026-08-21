namespace Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public required string ActionName { get; set; }
    public required string IpAddress { get; set; }
    public required string Details { get; set; }
    public bool Success { get; set; }
    public DateTimeOffset Time { get; set; }
}
