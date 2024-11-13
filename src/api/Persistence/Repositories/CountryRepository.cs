using api.Domain.Models;
using api.Domain.Repositories;
using api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace api.Persistence.Repositories
{
    public class CountryRepository : BaseRepository, ICountryRepository
    {
        public CountryRepository(AppDbContext context) : base(context) { }

        public async Task<Country> ListByNameAsync(string name)
        {
            return await _context.Countries
                           .Where(p => p.Name == name)
                           .FirstOrDefaultAsync();
        }

        public async Task<Country> SaveAsync(Country country)
        {            
            await _context.Countries.AddAsync(country);
            
            await _context.SaveChangesAsync();
            
            return country;
        }        
    }
}
