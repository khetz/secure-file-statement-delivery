using Infrastructure.Configuration;
using SecureFileStatementAPI.Configuration;
using SecureFileStatementAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .BindConfig<DataBaseConfig>(builder.Configuration, DataBaseConfig.DatabaseSectionName)
    .BindConfig<FileStorageConfig>(builder.Configuration, FileStorageConfig.StorageSectionName)
    .BindConfig<JwtAuthenticationConfig>(builder.Configuration, JwtAuthenticationConfig.JwtAuthenticationSectionName)
    .AddDatabase()
    .AddJWTAuthentication(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.Run();

