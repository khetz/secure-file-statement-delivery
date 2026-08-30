using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _appDbContext;
    public AuditLogRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _appDbContext.AddAsync(auditLog);
        await _appDbContext.SaveChangesAsync();
    }
}
