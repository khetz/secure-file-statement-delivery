using Infrastructure.Configuration;
using SecureFileStatementAPI.Configuration;
using SecureFileStatementAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .BindConfig<DataBaseConfig>(builder.Configuration, DataBaseConfig.DatabaseSectionName)
    .BindConfig<FileStorageConfig>(builder.Configuration, FileStorageConfig.StorageSectionName)
    .AddDatabase();

var app = builder.Build();
app.Run();

