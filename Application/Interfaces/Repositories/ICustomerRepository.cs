using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task AddAsync(Customer customer);
}