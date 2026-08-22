using Application.Interfaces.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly FileStorageConfig _fileStorageConfig;
    private readonly string _basePath;

    public FileStorageService(IOptions<FileStorageConfig> fileStorageConfig)
    {
        _fileStorageConfig = fileStorageConfig.Value;
        _basePath = _fileStorageConfig.BasePath ?? throw new ArgumentNullException("Base path not configured");

        Directory.CreateDirectory(_basePath);
    }

    public Task<bool> DeleteAsync(string storagePath)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string storagePath)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> RetrieveAsync(string storagePath)
    {
        throw new NotImplementedException();
    }

    public Task<string> StoreAsync(Stream fileContent, string storagePath)
    {
        throw new NotImplementedException();
    }
}
