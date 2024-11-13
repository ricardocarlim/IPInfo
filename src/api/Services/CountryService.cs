using api.Domain.Models;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Domain.Services.Communication;

namespace api.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        public CountryService(ICountryRepository countryRepository) { 
            _countryRepository = countryRepository;
        }
        public async Task<Country> ListByNameAsync(string name)
        {
            return await _countryRepository.ListByNameAsync(name);
        }

        public async Task<Country> SaveAsync(Country country)
        {
            return await _countryRepository.SaveAsync(country);            
        }
    }
}
