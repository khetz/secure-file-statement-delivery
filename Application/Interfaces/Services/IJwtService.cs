using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IJwtService
{
    (string, DateTimeOffset) GenerateToken(Customer customer);
}
