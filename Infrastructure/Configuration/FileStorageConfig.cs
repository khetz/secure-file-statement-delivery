namespace Infrastructure.Configuration;

public class FileStorageConfig
{
    public const string StorageSectionName = "FileStorage";
    public string BasePath { get; set; } = string.Empty;
}
