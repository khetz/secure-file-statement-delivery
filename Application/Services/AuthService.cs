using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Requests;
using Application.Responses;
using Domain.Entities;
using ErrorOr;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public AuthService(ICustomerRepository customerRepository, IJwtService jwtService, IPasswordService passwordService)
    {
        _customerRepository = customerRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest loginRequest)
    {
        var customer = await _customerRepository.GetByEmailAsync(loginRequest.Email);

        if (customer == null)
            return Error.Unauthorized(description: "Invalid credentials");

        if (!_passwordService.VerifyPassword(customer.PasswordHash, loginRequest.Password))
            return Error.Unauthorized(description: "Invalid credentials");

        var (accessToken, expiry) = _jwtService.GenerateToken(customer);

        return new AuthResponse()
        {
            Token = accessToken,
            TokenType = "Bearer",
            Expiry = expiry
        };
    }

    public async Task<ErrorOr<AuthResponse>> RegisterAsync(RegisterRequest registerRequest)
    {
        var trimmedEmail = registerRequest.Email.Trim();
        var customerExists = await _customerRepository.ExistsAsync(trimmedEmail);

        if (customerExists) return Error.Conflict("Customer exists.");
        if (registerRequest.Password.Length < 8) return Error.Validation("Password must at least be 8 characters long.");

        var passwordHash = _passwordService.HashPassword(registerRequest.Password);

        var customer = new Customer()
        {
            Email = registerRequest.Email,
            PasswordHash = passwordHash,
            FullName = registerRequest.FullName,
            AccountNumber = registerRequest.AccountNumber,
            Active = true
        };

        await _customerRepository.AddAsync(customer);
        var (accessToken, expiry) = _jwtService.GenerateToken(customer);

        return new AuthResponse()
        {
            Token = accessToken,
            TokenType = "Bearer",
            Expiry = expiry
        };
    }
}
