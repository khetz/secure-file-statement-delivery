using Application.Interfaces.Services;
using Application.Responses;
using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SecureFileStatementAPI.Extensions;
using System.Security.Claims;

namespace SecureFileStatementAPI.Endpoints.v1;

public static class StatementEndpoints
{
    public static void MapStatementEndpoints(this RouteGroupBuilder group)
    {
        var statementsGroup = group.MapGroup("statements");

        statementsGroup.MapPost("", UploadHandlerAsync);
    }

    private static async Task<Results<Created<StatementResponse>, BadRequest<string>, NotFound>> UploadHandlerAsync([FromForm] string period,
        [FromForm] IFormFile file, ClaimsPrincipal customer, [FromServices] IStatementService statementServive)
    {
        var customerId = customer.GetCustomerId();
        if (customerId == null) return TypedResults.NotFound();

        var result = await statementServive.UploadAsync((Guid)customerId, file, period);

        if (result.IsError)
        {
            var error = result.FirstError;
            return error.Type switch
            {
                ErrorType.NotFound => TypedResults.NotFound(),
                _ => TypedResults.BadRequest($"{error.Description}")
            };
        }

        return TypedResults.Created((string?)null, result.Value);   
    }
}
