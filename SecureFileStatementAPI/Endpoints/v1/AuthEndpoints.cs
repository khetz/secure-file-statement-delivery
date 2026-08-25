using Application.Interfaces.Services;
using Application.Requests;
using Application.Responses;
using ErrorOr;
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
            authGroup.MapPost("register", RegisterHandlerAsync);
        }

        private static async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>>
            LoginHandlerAsync([FromBody] LoginRequest loginRequest, [FromServices] IAuthService authService)
        {
            var result = await authService.LoginAsync(loginRequest);

            return result.Match<Results<Ok<AuthResponse>, UnauthorizedHttpResult>>(
                authResponse => TypedResults.Ok(authResponse),
                errors => TypedResults.Unauthorized());
        }

        private static async Task<Results<Created<AuthResponse>, Conflict, ValidationProblem>>
            RegisterHandlerAsync([FromBody] RegisterRequest registerRequest, [FromServices] IAuthService authService)
        {
            var result = await authService.RegisterAsync(registerRequest);

            if (result.IsError)
            {
                var error = result.FirstError;
                return error.Type switch
                {
                    ErrorType.Conflict => TypedResults.Conflict(),
                    _ => TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "error", new[] { error.Description } }
            })
                };
            }

            return TypedResults.Created((string?)null, result.Value);
        }
    }
}
