using api.Domain.Models;

namespace api.Domain.Services
{
    public interface ICountryService
    {        
        Task<Country> ListByNameAsync(string name);
        Task<Country> SaveAsync(Country country);        
    }
}
