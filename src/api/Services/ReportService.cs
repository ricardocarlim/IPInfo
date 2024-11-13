using api.Domain.Models;
using api.Domain.Repositories;
using api.Domain.Services;

namespace api.Services
{
    public class ReportService : IReportService
    {   
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository) {
            _reportRepository = reportRepository;
        }
        public Task<IEnumerable<CountryReport>> GetCountryReportAsync()
        {
            return _reportRepository.GetCountryReportAsync();
        }
    }
}
