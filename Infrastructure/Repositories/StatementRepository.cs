using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StatementRepository : IStatementRepository
{
    private readonly AppDbContext _appDbContext;

    public StatementRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Guid> AddAsync(Statement statement)
    {
        await _appDbContext.AddAsync(statement);
        await _appDbContext.SaveChangesAsync();
        return statement.Id;
    }

    public async Task<IReadOnlyCollection<Statement>> GetByCustomerIdAsync(Guid customerId)
    {
        var statements = _appDbContext.Statement.Where(s => s.CustomerId == customerId);
        return await statements.ToListAsync();
    }

    public async Task<Statement?> GetByIsAsync(Guid statementId)
    {
        return await _appDbContext.Statement.FirstOrDefaultAsync(s => s.Id == statementId);
    }
}
