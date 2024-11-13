using api.Domain.Models;
using api.Domain.Models.Queries;

namespace api.Domain.Repositories
{
    public interface IIPAddressRepository
    {
        Task<QueryResult<IPAddress>> ListAsync(IPAddressQuery query);
        Task<IPAddress> AddAsync(IPAddress ipaddress);
        Task<IPAddress> FindByIPAsync(string ip);
        void Update(IPAddress ipaddress);
        void Remove(IPAddress ipaddress);
    }
}
