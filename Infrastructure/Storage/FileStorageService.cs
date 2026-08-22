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

    public async Task<bool> DeleteAsync(string storagePath)
    {
        var fullFilePath = VerifyFilePath(storagePath);
        var fileExists = File.Exists(fullFilePath);

        if (!fileExists) return false;

        File.Delete(fullFilePath);

        return true;
    }

    public async Task<bool> ExistsAsync(string storagePath)
    {
        var fullFilePath = VerifyFilePath(storagePath);
        return await Task.FromResult(File.Exists(fullFilePath));
    }

    public async Task<Stream> RetrieveAsync(string storagePath)
    {
        var fullFilePath = VerifyFilePath(storagePath);
        var fileExists = File.Exists(fullFilePath);

        if (!fileExists) throw new FileNotFoundException($"File with storage path {storagePath} was not found");

        return new FileStream(
            fullFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
    }

    public async Task<string> StoreAsync(Stream fileContent, string storagePath)
    {
        var sanitisedFileName = Path.GetFileName(storagePath);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitisedFileName}";

        var fullFilePath = VerifyFilePath(storagePath + uniqueFileName);

        await using var fileStream = new FileStream(
            fullFilePath,
            FileMode.Create,
            FileAccess.Write);

        await fileContent.CopyToAsync(fileStream);
        return uniqueFileName;
    }

    private string VerifyFilePath(string storagePath)
    {
        var fullFilePath = Path.Combine(_basePath, storagePath);
        var valid = fullFilePath.StartsWith(_basePath);

        if (!valid) throw new Exception("Invalid file path specified");

        return fullFilePath;
    }
}