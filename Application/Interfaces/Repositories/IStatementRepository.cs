using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStatementRepository
{
    Task<Guid> AddAsync(Statement statement);
}
