using Application.Configuration;
using Application.Helpers;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtAuthenticationConfig _jwtAuthenticationConfig;

    public JwtService(IOptions<JwtAuthenticationConfig> jwtAuthenticationConfig)
    {
        _jwtAuthenticationConfig = jwtAuthenticationConfig.Value;
    }

    public string GenerateToken(Customer customer)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var secret = JwtHelper.GetSigningKey(_jwtAuthenticationConfig.Secret);
        var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtAuthenticationConfig.Issuer,
            audience: _jwtAuthenticationConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtAuthenticationConfig.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
