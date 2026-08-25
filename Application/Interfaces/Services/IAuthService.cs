using Application.Requests;
using Application.Responses;
using ErrorOr;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest loginRequest);
}
