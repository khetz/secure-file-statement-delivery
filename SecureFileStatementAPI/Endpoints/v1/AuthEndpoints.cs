using Application.Interfaces.Services;
using Application.Requests;
using Application.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SecureFileStatementAPI.Endpoints.v1
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this RouteGroupBuilder group)
        {
            var authGroup = group.MapGroup("auth");

            authGroup.MapPost("login", LoginHandlerAsync);
        }

        private static async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>>
            LoginHandlerAsync([FromBody] LoginRequest loginRequest, [FromServices] IAuthService authService)
        {
            var result = await authService.LoginAsync(loginRequest);

            return result.Match<Results<Ok<AuthResponse>, UnauthorizedHttpResult>>(
                authResponse => TypedResults.Ok(authResponse),
                errors => TypedResults.Unauthorized());
        }
    }
}
