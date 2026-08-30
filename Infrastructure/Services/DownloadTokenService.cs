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
        _downloadTokenRepository = downloadTokenRepository;
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
            IpAddress = ipAddress,
            Time = DateTimeOffset.UtcNow
        };

        await _auditLogRepository.AddAsync(auditLog);

        return new DownloadLinkResponse()
        {
            DownloadUrl = $"{_downloadTokenConfig.BaseUrl}/api/v1/statements/download",
            Expiry = expiryTimestamp
        };
    }

    public async Task<ErrorOr<DownloadValidationResponse>> ValidateTokenAsync(string token)
    {
        string[] tokenParts = token.Split(".", StringSplitOptions.None);
        if (tokenParts.Any(string.IsNullOrEmpty)) return Error.Unauthorized("Token not valid");

        var decodedPayload = FromBase64Url(tokenParts[0]);
        var decodedPayloadHash = HMACSigning(decodedPayload, _downloadTokenConfig.SigningKey);

        var decodedSignature = FromBase64Url(tokenParts[1]);
        var signaturesEqual = CryptographicOperations.FixedTimeEquals(decodedPayloadHash, decodedSignature);

        if (!signaturesEqual) return Error.Unauthorized("Token not valid");

        var payloadString = Encoding.UTF8.GetString(decodedPayload);
        var payloadParts = payloadString.Split("|", StringSplitOptions.None);

        if (payloadParts.Any(string.IsNullOrEmpty)) return Error.Unauthorized("Token not valid");

        var expiryTimestamp = DateTimeOffset.Parse(payloadParts[2]);

        if (DateTimeOffset.UtcNow > expiryTimestamp) return Error.Unauthorized("Download link has expired");

        var databaseToken = await _downloadTokenRepository.GetByTokenAsync(token);
        if (databaseToken == null || databaseToken.Used) return Error.Unauthorized("Invalid or expired token");

        return new DownloadValidationResponse()
        {
            CustomerId = Guid.Parse(payloadParts[1]),
            StatementId = Guid.Parse(payloadParts[0])
        };
    }

    public async Task<ErrorOr<bool>> MarkAsUsedAsync(string token)
    {
        var markedAsUsed = await _downloadTokenRepository.MarkAsUsedAsync(token);
        if (!markedAsUsed) return Error.NotFound("Token not found");

        return true;
    }

    #region private helpers

    private static byte[] HMACSigning(byte[] input, string secretKey)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        using var hmac = new HMACSHA256(keyBytes);
        return hmac.ComputeHash(input);
    }

    private static string BuildToken(string message, string secretKey)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        byte[] hashBytes = HMACSigning(messageBytes, secretKey);

        string payloadBase64 = ToBase64Url(messageBytes);
        string signatureBase64 = ToBase64Url(hashBytes);
        return $"{payloadBase64}.{signatureBase64}";
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private byte[] FromBase64Url(string base64Url)
    {
        var decodedString = base64Url.Replace("-", "+").Replace("_", "/");
        switch (decodedString.Length % 4) { case 2: decodedString += "=="; break; case 3: decodedString += "="; break; }
        return Convert.FromBase64String(decodedString);
    }
    #endregion
}
