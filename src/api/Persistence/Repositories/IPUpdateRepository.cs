using api.Domain.Models;
using api.Domain.Repositories;
using api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace api.Persistence.Repositories
{
    public class IPUpdateRepository : BaseRepository, IIPUpdateRepository
    {
        public IPUpdateRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<IPAddress>> GetIPsBatchAsync(int pageIndex, int pageSize)
        {
            return await _context.IPAddresses
               .Include(p => p.Country)
               .Skip(pageIndex * pageSize)
               .Take(pageSize)               
               .ToListAsync();
        }

        public async Task UpdateIPInfoAsync(IPAddress ipAddress)
        {
            _context.IPAddresses.Update(ipAddress);
            await _context.SaveChangesAsync();
        }
    }
}
