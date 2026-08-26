using Application.Responses;
using Domain.Entities;

namespace Infrastructure.Mappers;

public static class StatementMappers
{
    public static StatementResponse ToStatementResponse(this Statement statement) => new()
    {
        Id = statement.Id,
        FileName = statement.FileName,
        Period = statement.PeriodCovered,
        FileSize = statement.FileSize,
        UploadDate = statement.UploadTimestamp,
        ContentHash = statement.ContentHash
    };
}
