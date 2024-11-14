using api.Domain.Models;
using api.Persistence.Contexts;
using api.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Repositories
{
    public class CountryRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;

        public CountryRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDb")
                        .Options;
            _context = new AppDbContext(_options);
        }

        private CountryRepository CreateRepository()
        {
            return new CountryRepository(_context);
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveCountryToDatabase()
        {
            var country = new Country
            {
                Name = "Brazil",
                ThreeLetterCode = "BRA",
                TwoLetterCode = "BR"
            };

            var repository = CreateRepository();

            var savedCountry = await repository.SaveAsync(country);
            await _context.SaveChangesAsync();
            
            Assert.NotNull(savedCountry);
            Assert.Equal("Brazil", savedCountry.Name);

            var savedEntity = await _context.Countries
                                            .FirstOrDefaultAsync(c => c.Name == "Brazil");
            Assert.NotNull(savedEntity);
            Assert.Equal("Brazil", savedEntity.Name);
        }
    }
}
