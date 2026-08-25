namespace Application.Responses;

public class AuthResponse
{
    public required string Token { get; set; }
    public DateTimeOffset Expiry { get; set; }
    public required string TokenType { get; set; }
}
