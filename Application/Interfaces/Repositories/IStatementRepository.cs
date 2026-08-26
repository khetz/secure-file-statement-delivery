using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStatementRepository
{
    Task AddAsync(Statement statement);
}
