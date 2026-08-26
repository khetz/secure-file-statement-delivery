using System.Security.Claims;

namespace SecureFileStatementAPI.Extensions;

public static class ClaimsExtensions
{
    public static Guid? GetCustomerId(this ClaimsPrincipal customer)
    {
        var customerId = customer.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (customerId == null) return null;

        var parsedCustomerId = Guid.Parse(customerId);
        return parsedCustomerId;
    }
}
