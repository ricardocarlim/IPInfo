using api.Domain.Models;
using api.Persistence.Contexts;
using api.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Repositories
{
    public class IPAddressRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;

        public IPAddressRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDb")
                        .Options;
            _context = new AppDbContext(_options);
        }

        private IPAddressRepository CreateRepository()
        {
            return new IPAddressRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddIPAddressToDatabase()
        {
            var ipAddress = new IPAddress
            {
                IP = "192.168.1.1",
                Country = new Country { Name = "Brazil", ThreeLetterCode = "BRA", TwoLetterCode = "BR" }
            };

            var repository = CreateRepository();
            
            var addedIPAddress = await repository.AddAsync(ipAddress);
            await _context.SaveChangesAsync();
            
            Assert.NotNull(addedIPAddress);
            Assert.Equal("192.168.1.1", addedIPAddress.IP);
            Assert.Equal("Brazil", addedIPAddress.Country.Name);

            var savedIPAddress = await _context.IPAddresses
                                                .FirstOrDefaultAsync(ip => ip.IP == "192.168.1.1");
            Assert.NotNull(savedIPAddress);
            Assert.Equal("Brazil", savedIPAddress.Country.Name);
        }

        [Fact]
        public async Task FindByIPAsync_ShouldReturnIPAddress_WhenExists()
        {            
            var ipAddress = new IPAddress
            {
                IP = "192.168.1.1",
                Country = new Country { Name = "Brazil", ThreeLetterCode = "BRA", TwoLetterCode = "BR" }
            };

            _context.IPAddresses.Add(ipAddress);
            await _context.SaveChangesAsync();

            var repository = CreateRepository();
            
            var result = await repository.FindByIPAsync("192.168.1.1");
            
            Assert.NotNull(result);
            Assert.Equal("192.168.1.1", result.IP);
            Assert.Equal("Brazil", result.Country.Name);
        }

        [Fact]
        public async Task FindByIPAsync_ShouldReturnNull_WhenNotFound()
        {            
            var repository = CreateRepository();
            
            var result = await repository.FindByIPAsync("192.168.1.1");
            
            Assert.Null(result);
        }

        [Fact]
        public async Task ListByNameAsync_ShouldReturnCountry_WhenExists()
        {
            var brazil = new Country { Name = "Brazil", ThreeLetterCode = "BRA", TwoLetterCode = "BR" };
            var argentina = new Country { Name = "Argentina", ThreeLetterCode = "ARG", TwoLetterCode = "AR" };

            _context.Countries.Add(brazil);
            _context.Countries.Add(argentina);
            await _context.SaveChangesAsync();

            var countryRepository = new CountryRepository(_context); 

            var country = await countryRepository.ListByNameAsync("Brazil");

            Assert.NotNull(country); 
            Assert.Equal("Brazil", country.Name); 
        }

        [Fact]
        public async Task SaveAsync_ShouldUpdateIPAddress_WhenExists()
        {            
            var ipAddress = new IPAddress
            {
                IP = "192.168.1.1",
                Country = new Country { Name = "Brazil", ThreeLetterCode = "BRA", TwoLetterCode = "BR" }
            };

            _context.IPAddresses.Add(ipAddress);
            await _context.SaveChangesAsync();

            var repository = CreateRepository();
            
            var existingIPAddress = await repository.FindByIPAsync("192.168.1.1");
            existingIPAddress.Country.Name = "Updated Brazil";
            await repository.SaveAsync(existingIPAddress);
            await _context.SaveChangesAsync();
            
            var updatedIPAddress = await _context.IPAddresses
                                                 .FirstOrDefaultAsync(ip => ip.IP == "192.168.1.1");
            Assert.NotNull(updatedIPAddress);
            Assert.Equal("Updated Brazil", updatedIPAddress.Country.Name);
        }
    }
}
