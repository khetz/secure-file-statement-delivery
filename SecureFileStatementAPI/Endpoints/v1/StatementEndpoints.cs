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
        var statementsGroup = group.MapGroup("statements").RequireAuthorization();

        statementsGroup.MapPost("", UploadHandlerAsync).DisableAntiforgery();
        statementsGroup.MapGet("", ListStatementsHandlerAsync);
        statementsGroup.MapGet("download", DownloadHandlerAsync)
            .AllowAnonymous().RequireRateLimiting("download-policy");
        statementsGroup.MapPost("{statementId}/download-link", GenerateLinkHandlerAsync);
    }

    private static async Task<Results<Created<StatementResponse>, BadRequest<string>, NotFound>> UploadHandlerAsync([FromForm] string period,
        IFormFile file, HttpContext httpContext, [FromServices] IStatementService statementServive)
    {
        var customerId = httpContext.User.GetCustomerId();
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

    private static async Task<Results<Ok<List<StatementResponse>>, UnauthorizedHttpResult>> ListStatementsHandlerAsync(ClaimsPrincipal customer,
        [FromServices] IStatementService statementService)
    {
        var customerId = customer.GetCustomerId();
        if (customerId == null) return TypedResults.Unauthorized();

        var result = await statementService.GetStatementsByCustomerIdAsync((Guid)customerId);

        return TypedResults.Ok(result.Value.ToList());
    }

    private static async Task<Results<Ok<DownloadLinkResponse>, NotFound, ForbidHttpResult, UnauthorizedHttpResult>> GenerateLinkHandlerAsync(ClaimsPrincipal customer,
        HttpContext context,[FromServices] IDownloadTokenService downloadTokenService, [FromRoute] Guid statementId)
    {
        var customerId = customer.GetCustomerId();
        if (customerId == null) return TypedResults.Unauthorized();

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "";
        var result = await downloadTokenService.GenerateAsync(statementId, (Guid)customerId, ipAddress);

        if (result.IsError)
        {
            var error = result.FirstError;
            return error.Type switch
            {
                ErrorType.Forbidden => TypedResults.Forbid(),
                ErrorType.NotFound => TypedResults.NotFound(),
                _ => TypedResults.Unauthorized()
            };
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<FileStreamHttpResult, UnauthorizedHttpResult, NotFound>> DownloadHandlerAsync
        ([FromQuery] string token, [FromServices] IDownloadTokenService downloadTokenService, [FromServices] IFileStorageService fileStorageService,
        [FromServices] IStatementService statementService)
    {
        var validToken = await downloadTokenService.ValidateTokenAsync(token);
        if (validToken.IsError) return TypedResults.Unauthorized();

        var statement = await statementService.GetStatementByIdAsync(validToken.Value.StatementId);
        if (statement.Value == null) return TypedResults.NotFound();

        var fileOnDisk = await fileStorageService.ExistsAsync(statement.Value.StoragePath);
        if (!fileOnDisk) return TypedResults.NotFound();

        var fileStream = await fileStorageService.RetrieveAsync(statement.Value.StoragePath);
        var markedAsUsed = await downloadTokenService.MarkAsUsedAsync(token);

        if (!markedAsUsed.Value) return TypedResults.NotFound();

        return TypedResults.File(fileStream, "application/pdf", statement.Value.FileName);
    }

}
