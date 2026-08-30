using Application.Responses;
using ErrorOr;

namespace Application.Interfaces.Services;

public interface IDownloadTokenService
{
    Task<ErrorOr<DownloadLinkResponse>> GenerateAsync(Guid statementId, Guid customerId, string ipAddress);
    Task<ErrorOr<DownloadValidationResponse>> ValidateTokenAsync(string token);
    Task<ErrorOr<bool>> MarkAsUsedAsync(string token);
}
