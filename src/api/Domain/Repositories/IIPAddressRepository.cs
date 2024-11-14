using api.Domain.Models;
using api.Domain.Models.Queries;

namespace api.Domain.Repositories
{
    public interface IIPAddressRepository
    {        
        Task<IPAddress> AddAsync(IPAddress ipaddress);
        Task<IPAddress> FindByIPAsync(string ip);             
    }
}
