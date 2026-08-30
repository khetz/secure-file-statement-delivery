namespace Application.Responses;

public class StatementResponse
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string Period { get; set; }
    public long FileSize { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public required string ContentHash { get; set; }
    public required string StoragePath { get; set; }
}
