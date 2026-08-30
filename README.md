# Secure File Statement Delivery API

A production-grade .NET API for storing customer account statements as PDF files and providing secure, time-limited download links. Built with clean architecture, JWT authentication, HMAC-signed download tokens, and Docker containerisation.

## Quick Start

```bash
docker compose up --build
```

The API starts on `http://localhost:8080` with two seeded test users:

| Email | Password | AccountNo |
|---|---|---|
| vukheta99@gmail.com | Password123! | 12 |
| jane.smith@example.com | Password456! | 13 |

Each user has pre-loaded sample PDF statements ready for download.

## Full Flow Example

```bash
# 1. Login
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"vukheta@gmail.com","password":"Password123!"}'

# 2. List your statements (use token from login response)
curl http://localhost:8080/api/v1/statements \
  -H "Authorization: Bearer <TOKEN>"

# 3. Generate a secure download link (use a statement ID from step 2)
curl -X POST http://localhost:8080/api/v1/statements/<STATEMENT_ID>/download-link \
  -H "Authorization: Bearer <TOKEN>"

# 4. Download the PDF (no auth needed — token is in the URL)
curl -o statement.pdf "<DOWNLOAD_URL_FROM_STEP_3>"
```

## Architecture

```
SecureFileStatementAPI/
  Configuration/
  Endpoints/
  Extensions/                  
  Middleware/                 
  Extensions/
  Statements/               

Application/                 
  Configuration/                  
  Helpers/                    
  Interfaces/
  Requests/                    
  Responses/
  Services/

Domain/                       
  Entities/ 

Infrastructure/               
  Configuration/
  Contexts/
  Data/
  Extensions/
  Mappers/
  Migrations/
  Repositories/
  Sample statements/
  Storage/
```

**Dependency flow:** API → Application → Domain ← Infrastructure

The API and Infrastructure layers depend on Application and Domain. Application and Domain have no outward dependencies. Infrastructure implements interfaces defined in Application, wired through dependency injection.

## API Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/v1/auth/login | No | Login and receive JWT |
| POST | /api/v1/auth/register | No | Register a new customer |
| POST | /api/v1/statements | JWT | Upload a PDF statement |
| GET | /api/v1/statements | JWT | List your statements |
| GET | /api/v1/statements/download?token=... | No | Download PDF via signed token |
| POST | /api/v1/statements/{statementId}/download-link | JWT | Generate a secure download link |


## Security Design

### Authentication
- JWT Bearer tokens for API authentication
- BCrypt password hashing with per-hash salts
- Signing key configurable via environment variables, validated on startup

### Secure Download Links
- **HMAC-SHA256 signed tokens** — the download token encodes statement ID, customer ID, and expiry timestamp, signed with a server-side secret key. Without the key, tokens cannot be forged.
- **Time-limited** — tokens expire after a configurable window (default 15 minutes)
- **Single-use** — each token is marked as used after the first download, preventing replay attacks
- **Constant-time signature comparison** — uses `CryptographicOperations.FixedTimeEquals` to prevent timing attacks that could leak signature bytes

### Authorization
- **Ownership verification** — customers can only access their own statements. The API checks that the statement's customer ID matches the authenticated user before generating a download link.
- **GUID identifiers** — all entity IDs are GUIDs, preventing enumeration attacks (IDOR). Sequential IDs would allow guessing other customers' statement IDs.

### File Validation
- **PDF magic byte verification** — uploaded files are validated by checking the first 4 bytes for the `%PDF` signature, not by trusting the file extension or content-type header
- **SHA256 content hashing** — each stored file's hash is recorded for integrity verification
- **Path traversal prevention** — all file paths are sanitised and validated to ensure they remain within the storage directory

### Audit Trail
- All security-relevant actions are logged: download link generation, successful downloads, access denials, and authentication attempts
- Logs include customer ID, statement ID, IP address, and timestamp
  
### Rate Limiting
- **Link generation** — fixed window policy limiting download link requests per customer, preventing a compromised account from generating excessive tokens
- **Downloads** — fixed window policy limiting download requests per IP address, preventing abuse of valid download links

## Error Handling
- Global exception handling middleware catches all unhandled exceptions and returns standardised ProblemDetails responses
- Internal details and stack traces are never exposed to the client
- All errors are logged with contextual information for debugging
  
## Storage Architecture

File storage is abstracted behind `IFileStorageService` with a local filesystem implementation. The interface supports:
- `StoreAsync` — store a file from a stream
- `RetrieveAsync` — retrieve a file as a stream
- `DeleteAsync` — remove a file
- `ExistsAsync` — check file existence

This abstraction allows swapping to Azure Blob Storage (with native SAS token support) or S3 without changing any business logic — only a new implementation class and a DI registration change.

## Configuration

All sensitive configuration is externalised via environment variables (set in docker-compose.yml):

| Variable | Purpose |
|---|---|
| Database__DefaultConnection | SQLite connection string |
| JwtAuthentication__Secret | JWT signing key (min 32 chars) |
| JwtAuthentication__Issuer | JWT issuer |
| JwtAuthentication__Audience | JWT audience |
| JwtAuthentication__ExpiryMinutes | JWT token lifetime |
| DownloadToken__SigningKey | HMAC signing key for download tokens |
| DownloadToken__ExpiryMinutes | Download link lifetime |
| DownloadToken__BaseUrl | Public-facing base URL for download links |
| FileStorage__BasePath | Directory for PDF storage |

## Docker

Multi-stage build: SDK image for build, ASP.NET runtime image for production. The container runs as a non-root user. Volumes persist the database and uploaded PDFs across restarts.

```bash
docker compose up --build    # Build and start
docker compose down          # Stop
docker compose down -v       # Stop and remove volumes (clears data)
```

## Production Considerations

If deploying to production, the following enhancements would be added:

- **Azure Blob Storage** with SAS tokens replacing local file storage
- **HTTPS enforcement** via reverse proxy (nginx, Azure App Gateway)
- **Key rotation** for JWT and HMAC signing keys without invalidating active tokens
- **Background job** to purge expired download tokens periodically
- **Structured logging** with Serilog sinks to a centralised logging service
- **Health check endpoint** for container orchestration liveness/readiness probes
- **Integration tests** proving: ownership boundary (customer A cannot access customer B), expired token rejection, tampered token rejection, invalid PDF rejection, and rate limit enforcement

## Tech Stack

- .NET 10, Minimal APIs, Clean Architecture
- Entity Framework Core with SQLite
- JWT Bearer Authentication (BCrypt password hashing)
- HMAC-SHA256 signed download tokens
- Docker (multi-stage build)
- ErrorOr for result handling
