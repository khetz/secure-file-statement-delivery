using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string storedHashedPassword, string incomingPassword)
    {
        return BCrypt.Net.BCrypt.Verify(incomingPassword, storedHashedPassword);
    }
}
