using Application.Responses;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;

public interface IStatementService
{
    Task<ErrorOr<StatementResponse>> UploadAsync(Guid customerId, IFormFile file, string period);
    Task<ErrorOr<IReadOnlyCollection<StatementResponse>>> GetStatementsByCustomerIdAsync(Guid customerId);
}
