using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DownloadTokenRepository : IDownloadTokenRepository
{
    private readonly AppDbContext _appDbContext;

    public DownloadTokenRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task AddAsync(DownloadToken token)
    {
        await _appDbContext.AddAsync(token);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<DownloadToken?> GetByTokenAsync(string token)
    {
        return await _appDbContext.DownloadTokens.FirstOrDefaultAsync(t => t.Token == token);
    }
}