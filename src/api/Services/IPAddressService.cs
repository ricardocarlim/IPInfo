using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Domain.Services.Communication;
using api.Infraestructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace api.Services
{
    public class IPAddressService : IIPAddressService
    {
        private readonly IIPAddressRepository _ipAddressRepository;
        private readonly ICountryService _countryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IPAddressService> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDistributedCache _distributedCache;
        private readonly IP2CCacheService _cacheService;

        public IPAddressService(
            IIPAddressRepository ipAddressRepository,
            ICountryService countryService,
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            ILogger<IPAddressService> logger,
            IConnectionMultiplexer connectionMultiplexer,
            IDistributedCache distributedCache,
            IP2CCacheService cacheService)
        {
            _ipAddressRepository = ipAddressRepository;
            _countryService = countryService;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
        }

        public async Task<IPAddress> FindByIPAsync(IPAddressQuery ipAddressQuery)
        {
            var ipAddress = await FindInCacheByIPAsync(ipAddressQuery.IP);
            if (ipAddress != null)
                return ipAddress;

            ipAddress = await FindInDBByIPAsync(ipAddressQuery.IP);
            if (ipAddress != null)
            {
                await _cacheService.AddIPToCacheAsync(ipAddressQuery.IP, ipAddress);
                return ipAddress;
            }

            return await CreateNewIPAddress(ipAddressQuery.IP);
        }

        public async Task<IPAddress> FindInCacheByIPAsync(string ip)
        {
            return await _cacheService.GetIPFromCacheAsync(ip);
        }

        public async Task<IPAddress> FindInDBByIPAsync(string ip)
        {
            return await _ipAddressRepository.FindByIPAsync(ip);
        }

        public async Task<Country> FindCountryInIP2CByIPAsync(string ip)
        {
            return await IP2CService.GetCountryInfoFromIPAsync(ip);
        }

        public async Task<IPAddressResponse> SaveAsync(IPAddress ipAddress)
        {
            try
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    ipAddress = PrepareNewIPAddress(ipAddress);
                    ipAddress = await SaveToDatabase(ipAddress);
                    await SaveToCache(ipAddress);
                    await _unitOfWork.CompleteAsync();

                    await transaction.CommitAsync();

                    return new IPAddressResponse(ipAddress);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving IP address: {ex.Message}");
                await _unitOfWork.RollbackAsync();
                return new IPAddressResponse($"Error saving IP address: {ex.Message}");
            }
        }

        private IPAddress PrepareNewIPAddress(IPAddress ipAddress)
        {
            ipAddress.CreatedAt = DateTime.UtcNow;
            ipAddress.UpdatedAt = DateTime.UtcNow.AddYears(-100);
            return ipAddress;
        }

        private async Task<IPAddress> SaveToDatabase(IPAddress ipAddress)
        {
            return await _ipAddressRepository.AddAsync(ipAddress);
        }

        private async Task SaveToCache(IPAddress ipAddress)
        {
            await _cacheService.AddIPToCacheAsync(ipAddress.IP, ipAddress);
        }

        private async Task<IPAddress> CreateNewIPAddress(string ip)
        {
            var ipAddress = new IPAddress
            {
                IP = ip,
                Country = await FindCountry(ip)
            };

            if (ipAddress.Country != null)
            {
                ipAddress.CreatedAt = DateTime.UtcNow;
                ipAddress.UpdatedAt = DateTime.UtcNow;

                ipAddress = await _ipAddressRepository.AddAsync(ipAddress);
                await _unitOfWork.CompleteAsync();

                await _cacheService.AddIPToCacheAsync(ip, ipAddress);
            }

            return ipAddress;
        }

        private async Task<Country> FindCountry(string ip)
        {
            var country = await IP2CService.GetCountryInfoFromIPAsync(ip);
            if (country != null)
            {
                var existingCountry = await _countryService.ListByNameAsync(country.Name);
                return existingCountry ?? await _countryService.SaveAsync(country);
            }

            throw new KeyNotFoundException("Country information not found for the specified IP.");
        }
    }
}
