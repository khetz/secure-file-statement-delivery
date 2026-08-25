using SecureFileStatementAPI.Endpoints.v1;

namespace SecureFileStatementAPI.Extensions
{
    public static class MapV1Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var v1Group = app.MapGroup("api/v1");

            v1Group.MapAuthEndpoints();
        }
    }
}
