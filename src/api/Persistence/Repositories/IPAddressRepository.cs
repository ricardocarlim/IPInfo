using api.Domain.Models.Queries;
using api.Domain.Repositories;
using api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace api.Persistence.Repositories
{
    public class IPAddressRepository : BaseRepository, IIPAddressRepository
    {
        public IPAddressRepository(AppDbContext context) : base(context) { }

        public async Task<Domain.Models.IPAddress> AddAsync(Domain.Models.IPAddress ipAddress)
        {
            await _context.IPAddresses.AddAsync(ipAddress);            
            await _context.SaveChangesAsync();
            return ipAddress;
        }

        public async Task<Domain.Models.IPAddress> FindByIPAsync(string ip)
        {
            var ret = await _context.IPAddresses
                            .Where(p => p.IP == ip)
                            .Include(p => p.Country)
                            .FirstOrDefaultAsync();

            return ret;
        }

        public Task<QueryResult<Domain.Models.IPAddress>> ListAsync(IPAddressQuery query)
        {
            throw new NotImplementedException();
        }

        public void Remove(Domain.Models.IPAddress ipaddress)
        {
            throw new NotImplementedException();
        }

        public void Update(Domain.Models.IPAddress ipaddress)
        {
            throw new NotImplementedException();
        }

       
    }
}
