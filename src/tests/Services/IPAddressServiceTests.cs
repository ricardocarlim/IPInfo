using Moq;
using api.Domain.Models;
using api.Domain.Services;
using api.Domain.Services.Communication;
using api.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using api.Domain.Models.Queries;
using api.Domain.Repositories;
using api.Infraestructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace api.Tests.Services
{
    public class IPAddressServiceTests
    {
        private readonly Mock<IIPAddressRepository> _mockIpAddressRepository;
        private readonly Mock<ICountryService> _mockCountryService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IP2CCacheService> _mockCacheService;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly Mock<ILogger<IPAddressService>> _mockLogger;
        private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
        private readonly Mock<IDistributedCache> _mockDistributedCache;

        private readonly IPAddressService _service;

        public IPAddressServiceTests()
        {
            _mockIpAddressRepository = new Mock<IIPAddressRepository>();
            _mockCountryService = new Mock<ICountryService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCacheService = new Mock<IP2CCacheService>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<IPAddressService>>();
            _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            _mockDistributedCache = new Mock<IDistributedCache>();

            _service = new IPAddressService(
                _mockIpAddressRepository.Object,
                _mockCountryService.Object,
                _mockUnitOfWork.Object,
                _mockMemoryCache.Object,
                _mockLogger.Object,
                _mockConnectionMultiplexer.Object,
                _mockDistributedCache.Object,
                _mockCacheService.Object
            );
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveIPAddressAndReturnResponse()
        {
            var ipAddress = new IPAddress
            {
                IP = "192.168.1.1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockIpAddressRepository.Setup(r => r.AddAsync(It.IsAny<IPAddress>())).ReturnsAsync(ipAddress);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);
            _mockCacheService.Setup(c => c.AddIPToCacheAsync(It.IsAny<string>(), It.IsAny<IPAddress>())).Returns(Task.CompletedTask);

            var result = await _service.SaveAsync(ipAddress);

            Assert.NotNull(result);
            Assert.Equal(ipAddress.IP, result.IPAddress.IP);
            _mockIpAddressRepository.Verify(r => r.AddAsync(It.IsAny<IPAddress>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
            _mockCacheService.Verify(c => c.AddIPToCacheAsync(It.IsAny<string>(), It.IsAny<IPAddress>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ShouldReturnErrorResponse_WhenExceptionOccurs()
        {
            var ipAddress = new IPAddress
            {
                IP = "192.168.1.1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockIpAddressRepository.Setup(r => r.AddAsync(It.IsAny<IPAddress>())).ThrowsAsync(new Exception("Database error"));

            var result = await _service.SaveAsync(ipAddress);

            Assert.NotNull(result);
            Assert.StartsWith("Error saving IP address:", result.Message);
        }

        [Fact]
        public async Task FindByIPAsync_ShouldReturnIPAddressFromCache_WhenAvailable()
        {
            var ip = "192.168.1.1";
            var cachedIPAddress = new IPAddress
            {
                IP = ip,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCacheService.Setup(c => c.GetIPFromCacheAsync(ip)).ReturnsAsync(cachedIPAddress);

            var result = await _service.FindByIPAsync(new IPAddressQuery { IP = ip });

            Assert.NotNull(result);
            Assert.Equal(ip, result.IP);
            _mockCacheService.Verify(c => c.GetIPFromCacheAsync(ip), Times.Once);
        }

        [Fact]
        public async Task FindByIPAsync_ShouldReturnNewIPAddress_WhenNotFoundInCacheOrDB()
        {
            var ip = "192.168.1.1";
            var country = new Country { Name = "Test Country", ThreeLetterCode = "TST", TwoLetterCode = "TC" };

            _mockCacheService.Setup(c => c.GetIPFromCacheAsync(ip)).ReturnsAsync((IPAddress)null);
            _mockIpAddressRepository.Setup(r => r.FindByIPAsync(ip)).ReturnsAsync((IPAddress)null);
            _mockCountryService.Setup(c => c.ListByNameAsync(country.Name)).ReturnsAsync(country);
            _mockIpAddressRepository.Setup(r => r.AddAsync(It.IsAny<IPAddress>())).ReturnsAsync(new IPAddress { IP = ip, Country = country });
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);
            _mockCacheService.Setup(c => c.AddIPToCacheAsync(ip, It.IsAny<IPAddress>())).Returns(Task.CompletedTask);

            var result = await _service.FindByIPAsync(new IPAddressQuery { IP = ip });

            Assert.NotNull(result);
            Assert.Equal(ip, result.IP);
            _mockIpAddressRepository.Verify(r => r.AddAsync(It.IsAny<IPAddress>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
            _mockCacheService.Verify(c => c.AddIPToCacheAsync(ip, It.IsAny<IPAddress>()), Times.Once);
        }

        [Fact]
        public async Task FindByIPAsync_ShouldReturnNull_WhenCountryNotFound()
        {
            var ip = "192.168.1.1";

            _mockCacheService.Setup(c => c.GetIPFromCacheAsync(ip)).ReturnsAsync((IPAddress)null);
            _mockIpAddressRepository.Setup(r => r.FindByIPAsync(ip)).ReturnsAsync((IPAddress)null);
            _mockCountryService.Setup(c => c.ListByNameAsync(It.IsAny<string>())).ReturnsAsync((Country)null);

            var result = await _service.FindByIPAsync(new IPAddressQuery { IP = ip });

            Assert.Null(result);
            _mockCacheService.Verify(c => c.GetIPFromCacheAsync(ip), Times.Once);
            _mockIpAddressRepository.Verify(r => r.FindByIPAsync(ip), Times.Once);
        }
    }
}
