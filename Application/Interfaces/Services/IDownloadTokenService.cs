using Application.Responses;
using ErrorOr;

namespace Application.Interfaces.Services;

public interface IDownloadTokenService
{
    Task<ErrorOr<DownloadLinkResponse>> GenerateAsync(Guid statementId, Guid customerId);
}
