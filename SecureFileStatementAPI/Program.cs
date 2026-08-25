using Application.Configuration;
using Infrastructure.Configuration;
using SecureFileStatementAPI.Configuration;
using SecureFileStatementAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .BindConfig<DataBaseConfig>(builder.Configuration, DataBaseConfig.DatabaseSectionName)
    .BindConfig<FileStorageConfig>(builder.Configuration, FileStorageConfig.StorageSectionName)
    .BindConfig<JwtAuthenticationConfig>(builder.Configuration, JwtAuthenticationConfig.JwtAuthenticationSectionName)
    .AddDatabase()
    .AddRepositories()
    .AddServices()
    .AddJWTAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.Run();

