namespace Application.Interfaces.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string storedHashedPassword, string incomingPassword);
}
