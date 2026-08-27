namespace Application.Responses;

public class DownloadLinkResponse
{
    public required string DownloadUrl { get; set; }
    public DateTimeOffset Expiry { get; set; }
}
