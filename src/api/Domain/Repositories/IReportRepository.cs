using api.Domain.Models;

namespace api.Domain.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<CountryReport>> GetCountryReportAsync();
    }
}
