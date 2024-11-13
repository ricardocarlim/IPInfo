﻿using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Domain.Services.Communication;
using api.Infraestructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using StackExchange.Redis;
using System;
using System.Data;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            var ipAddress = await TryFindInCache(ipAddressQuery.IP);
            if (ipAddress != null)
                return ipAddress;

            ipAddress = await TryFindInDB(ipAddressQuery.IP);
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
                ipAddress.CreatedAt = DateTime.UtcNow;
                ipAddress.UpdatedAt = DateTime.UtcNow.AddYears(-100);
                ipAddress = await _ipAddressRepository.AddAsync(ipAddress);
                await _unitOfWork.CompleteAsync();

                await _cacheService.AddIPToCacheAsync(ipAddress.IP, ipAddress);

                return new IPAddressResponse(ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving IP address: {ex.Message}");
                return new IPAddressResponse($"Error saving IP address: {ex.Message}");
            }
        }

        public async Task<IPAddressResponse> UpdateAsync(IPAddress ipAddress)
        {
            try
            {
                var existingIp = await _ipAddressRepository.FindByIPAsync(ipAddress.IP);
                if (existingIp == null)
                    return new IPAddressResponse("IP address not found");

                existingIp.IP = ipAddress.IP;
                existingIp.Country = ipAddress.Country;
                existingIp.UpdatedAt = DateTime.UtcNow;

                _ipAddressRepository.Update(existingIp);
                await _unitOfWork.CompleteAsync();

                await _cacheService.AddIPToCacheAsync(existingIp.IP, existingIp);

                return new IPAddressResponse(existingIp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating IP address: {ex.Message}");
                return new IPAddressResponse($"Error updating IP address: {ex.Message}");
            }
        }

        public async Task<IPAddressResponse> DeleteAsync(string ip)
        {
            try
            {
                var ipAddress = await _ipAddressRepository.FindByIPAsync(ip);
                if (ipAddress == null)
                    return new IPAddressResponse("IP address not found");

                _ipAddressRepository.Remove(ipAddress);
                await _unitOfWork.CompleteAsync();

                await _cacheService.RemoveIPFromCacheAsync(ipAddress.IP);

                return new IPAddressResponse(ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting IP address: {ex.Message}");
                return new IPAddressResponse($"Error deleting IP address: {ex.Message}");
            }
        }

        private async Task<Domain.Models.IPAddress> TryFindInCache(string ip)
        {
            return await _cacheService.GetIPFromCacheAsync(ip);
        }

        private async Task<Domain.Models.IPAddress> TryFindInDB(string ip)
        {
            return await _ipAddressRepository.FindByIPAsync(ip);
        }

        private async Task<Domain.Models.IPAddress> CreateNewIPAddress(string ip)
        {
            var ipAddress = new Domain.Models.IPAddress
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
            }else
            {
                throw new KeyNotFoundException("Country information not found for the specified IP.");
            }
            
        }
    }
}