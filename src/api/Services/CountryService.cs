using api.Domain.Models;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Domain.Services.Communication;
using api.Persistence;

namespace api.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(ICountryRepository countryRepository, IUnitOfWork unitOfWork)
        {
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Country> ListByNameAsync(string name)
        {
            return await _countryRepository.ListByNameAsync(name);
        }

        public async Task<Country> SaveAsync(Country country)
        {
            try
            {
                // Inicia a transação usando o UnitOfWork, que já tem a implementação de Transaction
                await _unitOfWork.BeginTransactionAsync();

                var existingCountry = await _countryRepository.ListByNameAsync(country.Name);

                await _countryRepository.SaveAsync(country);
                await _unitOfWork.CompleteAsync();

                // Commit da transação
                await _unitOfWork.CommitAsync();

                return country;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Error saving country", ex);
            }
        }

    }
}
