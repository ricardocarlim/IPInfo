using api.Domain.Models;

namespace api.Domain.Services
{
    public interface IReportService
    {
        Task<IEnumerable<CountryReport>> GetCountryReportAsync();
    }
}
