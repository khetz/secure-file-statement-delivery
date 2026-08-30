using Application.Configuration;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Responses;
using Domain.Entities;
using ErrorOr;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class DownloadTokenService : IDownloadTokenService
{
    private readonly IStatementService _statementService;
    private readonly DownloadTokenConfig _downloadTokenConfig;
    private readonly IDownloadTokenRepository _downloadTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public DownloadTokenService(IStatementService statementService, IOptions<DownloadTokenConfig> downloadTokenConfig, IDownloadTokenRepository downloadTokenRepository,
        IAuditLogRepository auditLogRepository)
    {
        _statementService = statementService;
        _downloadTokenConfig = downloadTokenConfig.Value;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<ErrorOr<DownloadLinkResponse>> GenerateAsync(Guid statementId, Guid customerId, string ipAddress)
    {
        var statement = await _statementService.GetStatementByIdAsync(statementId);
        if (statement.Value == null) return Error.NotFound("Statement does not exist");

        if (statement.Value.CustomerId != customerId)
            return Error.Forbidden("This statement does not belong to this customer.");

        var expiryTimestamp = DateTimeOffset.UtcNow.AddMinutes(_downloadTokenConfig.ExpiryMinutes);
        var tokenPayload = $"{statementId}|{customerId}|{expiryTimestamp.ToString("o")}";

        var token = BuildToken(tokenPayload, _downloadTokenConfig.SigningKey);

        var downloadToken = new DownloadToken()
        {
            Token = token,
            StatementId = statementId,
            CustomerId = customerId,
            CreationTime = DateTimeOffset.UtcNow,
            ExpirationTime = expiryTimestamp,
            Used = false
        };

        await _downloadTokenRepository.AddAsync(downloadToken);

        var auditLog = new AuditLog()
        {
            ActionName = "DownloadLinkGenerated",
            Details = $"CustomerId = {customerId}, statementId = {statementId}",
            IpAddress = ipAddress
        };

        await _auditLogRepository.AddAsync(auditLog);

        return new DownloadLinkResponse()
        {
            DownloadUrl = $"{_downloadTokenConfig.BaseUrl}",
            Expiry = expiryTimestamp
        };
    }

    private static string BuildToken(string message, string secretKey)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);

        byte[] hashBytes = hmac.ComputeHash(messageBytes);

        string payloadBase64 = ToBase64Url(messageBytes);
        string signatureBase64 = ToBase64Url(hashBytes);
        return $"{payloadBase64}.{signatureBase64}";
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
