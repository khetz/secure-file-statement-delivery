namespace Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> StoreAsync(Stream fileContent, string fileName);
    Task<Stream> RetrieveAsync(string storagePath);
    Task<bool> DeleteAsync(string storagePath);
    Task<bool> ExistsAsync(string storagePath);
}
