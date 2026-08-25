using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _appDbContext;

    public CustomerRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _appDbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _appDbContext.Customers.AnyAsync(c => c.Email == email);
    }

    public async Task AddAsync(Customer customer)
    {
        await _appDbContext.Customers.AddAsync(customer);
        await _appDbContext.SaveChangesAsync();
    }
}
