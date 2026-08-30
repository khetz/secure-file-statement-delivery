# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching
COPY Domain/*.csproj Domain/
COPY Application/*.csproj Application/
COPY Infrastructure/*.csproj Infrastructure/
COPY SecureFileStatementAPI/*.csproj SecureFileStatementAPI/
COPY SecureFileStatementAPI.slnx .
RUN dotnet restore SecureFileStatementAPI/SecureFileStatementAPI.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish SecureFileStatementAPI/SecureFileStatementAPI.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user
RUN useradd -m appuser

# Create directories for storage and database
RUN mkdir -p /app/Statements /app/Data && chown -R appuser:appuser /app

COPY --from=build /app/publish .

# Switch to non-root user
USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SecureFileStatementAPI.dll"]
