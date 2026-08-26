using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Contexts;

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
}
