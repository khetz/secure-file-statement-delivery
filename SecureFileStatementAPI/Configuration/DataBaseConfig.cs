namespace SecureFileStatementAPI.Configuration;

public class DataBaseConfig
{
    public const string DatabaseSectionName = "Database";
    public string DefaultConnection { get; set; } = string.Empty;
}
