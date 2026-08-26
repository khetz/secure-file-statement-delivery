using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStatementRepository
{
    Task<Guid> AddAsync(Statement statement);
    Task<IReadOnlyCollection<Statement>> GetByCustomerIdAsync(Guid customerId);
    Task<Statement?> GetByIsAsync(Guid statementId);
}
