using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Domain.Services.Communication;

namespace api.Domain.Services
{
    public interface IIPAddressService
    {        
        Task<IPAddress> FindInCacheByIPAsync(string ip);
        Task<IPAddress> FindInDBByIPAsync(string ip);
        Task<Country> FindCountryInIP2CByIPAsync(string ip);
        Task<IPAddress> FindByIPAsync(IPAddressQuery ipAddressQuery);
        Task<IPAddressResponse> SaveAsync(Models.IPAddress ipAddress);
        Task<IPAddressResponse> UpdateAsync(Models.IPAddress ipAddress);
        Task<IPAddressResponse> DeleteAsync(string ip);
    }
}
