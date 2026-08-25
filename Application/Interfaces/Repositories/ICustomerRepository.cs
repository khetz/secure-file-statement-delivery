using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email);
}
