using api.Domain.Models;

namespace api.Domain.Repositories
{
    public interface IIPUpdateRepository
    {
        Task<IEnumerable<IPAddress>> GetIPsBatchAsync(int pageIndex, int pageSize);

        Task UpdateIPInfoAsync(IPAddress ip);
    }
}
