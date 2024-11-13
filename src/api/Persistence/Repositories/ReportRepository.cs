using api.Domain.Models;
using api.Domain.Repositories;
using api.Persistence.Contexts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace api.Persistence.Repositories
{
    public class ReportRepository : BaseRepository, IReportRepository
    {
        public ReportRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CountryReport>> GetCountryReportAsync()
        {
            var query = @"
                SELECT 
                    c.Name AS CountryName,
                    COUNT(ipa.Id) AS AddressesCount,
                    MAX(ipa.UpdatedAt) AS LastAddressUpdated
                FROM 
                    dbo.Countries c
                LEFT JOIN 
                    dbo.IPAddresses ipa ON c.Id = ipa.CountryId
                GROUP BY 
                    c.Name
                ORDER BY 
                    c.Name;
            ";

            // Executando a consulta SQL pura e retornando os resultados diretamente
            var result = await _context.Set<CountryReport>().FromSqlRaw(query).ToListAsync();
            return result;            
        }
    }
}
