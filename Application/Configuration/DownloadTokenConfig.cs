namespace Application.Configuration;

public class DownloadTokenConfig
{
    public const string DowloadTokenSectionName = "DownloadToken";
    public string SigningKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
    public string BaseUrl { get; set; }
}
