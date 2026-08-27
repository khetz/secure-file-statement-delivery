using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Responses;
using Domain.Entities;
using ErrorOr;
using Infrastructure.Mappers;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace Infrastructure.Services;

public class StatementService : IStatementService
{
    private readonly ICustomerRepository _customerRespository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IStatementRepository _statementRepository;

    public StatementService(ICustomerRepository customerRepository, IFileStorageService fileStorageService, IStatementRepository statementRepository)
    {
        _customerRespository = customerRepository;
        _fileStorageService = fileStorageService;
        _statementRepository = statementRepository;
    }

    public async Task<ErrorOr<StatementResponse>> UploadAsync(Guid customerId, IFormFile file, string period)
    {
        var customerExists = await _customerRespository.ExistsAsync(customerId);
        if (!customerExists) return Error.NotFound("Customer does not exist.");

        if (file.Length > (10 * 1024 * 1024)) return Error.Validation("File Exceeds 10MB limit.");

        if (!IsPdf(file)) return Error.Validation("File is not a valid PDF.");

        var hexHash = await ComputeFileHash(file);

        using var stream = file.OpenReadStream();
        var storagePath = await _fileStorageService.StoreAsync(stream, file.FileName);

        var statement = new Statement()
        {
            CustomerId = customerId,
            FileName = file.FileName,
            StoragePath = storagePath,
            PeriodCovered = period,
            FileSize = file.Length,
            ContentHash = hexHash,
            UploadTimestamp = DateTimeOffset.UtcNow
        };

        var statementId = await _statementRepository.AddAsync(statement);

        return new StatementResponse()
        {
            Id = statementId,
            FileName = file.FileName,
            Period = period,
            FileSize = file.Length,
            UploadDate = statement.UploadTimestamp,
            ContentHash = hexHash
        };
    }
    
    public async Task<ErrorOr<IReadOnlyCollection<StatementResponse>>> GetStatementsByCustomerIdAsync(Guid customerId)
    {
        var statements = await _statementRepository.GetByCustomerIdAsync(customerId);

        return statements.Select(s => s.ToStatementResponse()).ToList();
    }

    public async Task<ErrorOr<StatementWithCustomerIdResponse?>> GetStatementByIdAsync(Guid statementId)
    {
        var statement = await _statementRepository.GetByIdAsync(statementId);
        return statement?.ToStatementWithCustomerIdResponse();
    }

    #region private functions
    private static bool IsPdf(IFormFile file)
    {
        if (file == null || file.Length < 4)
            return false;

        using var stream = file.OpenReadStream();
        byte[] header = new byte[4];
        int bytesRead = stream.Read(header, 0, 4);

        if (bytesRead < 4)
            return false;

        return header[0] == 0x25 &&
               header[1] == 0x50 &&
               header[2] == 0x44 &&
               header[3] == 0x46;
    }

    private static async Task<string> ComputeFileHash(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var sha256Instance = SHA256.Create();
        var fileHash = await sha256Instance.ComputeHashAsync(stream);

        return Convert.ToHexString(fileHash);
    }
    #endregion
}
