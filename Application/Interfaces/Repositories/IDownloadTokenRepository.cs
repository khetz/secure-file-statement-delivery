using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IDownloadTokenRepository
{
    Task AddAsync(DownloadToken token);
}
