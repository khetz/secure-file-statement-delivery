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

    public async Task<Stream> RetrieveAsync(string fileName)
    {
        var fullFilePath = Path.Combine(_basePath, fileName);
        VerifyFilePath(fullFilePath);
        var fileExists = File.Exists(fullFilePath);

        if (!fileExists) throw new FileNotFoundException($"File with name {fileName} was not found");

        return new FileStream(
            fullFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
    }

    public async Task<string> StoreAsync(Stream fileContent, string fileName)
    {
        var sanitisedFileName = Path.GetFileName(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitisedFileName}";
        var fullFilePath = Path.Combine(_basePath, uniqueFileName);

        VerifyFilePath(fullFilePath);

        await using var fileStream = new FileStream(
            fullFilePath,
            FileMode.Create,
            FileAccess.Write);

        await fileContent.CopyToAsync(fileStream);
        return uniqueFileName;
    }

    private void VerifyFilePath(string filePath)
    {
        var fullInputPath = Path.GetFullPath(filePath);
        var valid = fullInputPath.StartsWith(_basePath);

        if (!valid) throw new Exception("Invalid file path specified");
    }
}
