using SecureFileStatementAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .BindAllConfigs(builder.Configuration)
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

