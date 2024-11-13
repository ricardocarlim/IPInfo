using api.Domain.Models;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Infraestructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;

namespace api.Services
{
    public class IPUpdateService : IIPUpdateService
    {
        private readonly IIPUpdateRepository _iIPUpdateRepository;
        private readonly ICountryService _iCountryService;
        private readonly IP2CCacheService _iP2CCacheService;

        private static readonly AsyncPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );

        public IPUpdateService(IIPUpdateRepository iIPUpdateRepository,
            IP2CCacheService iP2CCacheService,
            ICountryService iCountryService)
        {
            _iIPUpdateRepository = iIPUpdateRepository;
            _iP2CCacheService = iP2CCacheService;
            _iCountryService = iCountryService;
        }

        public async Task UpdateIPsAsync()
        {
            int pageSize = 100;
            int pageIndex = 0;
            IEnumerable<IPAddress> batch;

            do
            {
                batch = await GetIPsBatchAsync(pageIndex, pageSize);

                foreach (var existingIP in batch)
                {
                    try
                    {
                        var updatedCountry = await RetryPolicy.ExecuteAsync(() => GetCountryInfoFromIPAsync(existingIP.IP));

                        if (HasCountryInfoChanged(existingIP.Country, updatedCountry))
                        {
                            await RetryPolicy.ExecuteAsync(() => UpdateIPInfoAsync(existingIP, updatedCountry));
                            await RetryPolicy.ExecuteAsync(() => UpdateCacheAsync(existingIP.IP, existingIP));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating IP {existingIP.IP}: {ex.Message}");
                    }
                }

                pageIndex++;

            } while (batch.Any());
        }

        private async Task<IEnumerable<IPAddress>> GetIPsBatchAsync(int pageIndex, int pageSize)
        {
            try
            {
                return await _iIPUpdateRepository.GetIPsBatchAsync(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching IPs batch {pageIndex}: {ex.Message}");
                throw;
            }
        }

        private async Task<Country> GetCountryInfoFromIPAsync(string ip)
        {
            try
            {
                return await IP2CService.GetCountryInfoFromIPAsync(ip);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching country information for IP {ip}: {ex.Message}");
                throw;
            }
        }

        private bool HasCountryInfoChanged(Country country, Country updatedCountry)
        {
            return country == null ||
                   country.Name != updatedCountry.Name ||
                   country.TwoLetterCode != updatedCountry.TwoLetterCode ||
                   country.ThreeLetterCode != updatedCountry.ThreeLetterCode;
        }

        private async Task UpdateIPInfoAsync(IPAddress existingIP, Country updatedCountry)
        {
            try
            {                
                var country = await _iCountryService.ListByNameAsync(updatedCountry.Name);
                if (country == null)
                {
                    country = await _iCountryService.SaveAsync(updatedCountry);
                    existingIP.Country = country;
                }
                existingIP.CountryId = country.Id;
                existingIP.UpdatedAt = DateTime.UtcNow;
                await _iIPUpdateRepository.UpdateIPInfoAsync(existingIP);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating IP information for {existingIP.IP}: {ex.Message}");
                throw;
            }
        }

        private async Task UpdateCacheAsync(string ip, IPAddress ipAddress)
        {
            try
            {
                await _iP2CCacheService.RemoveIPFromCacheAsync(ip);
                await _iP2CCacheService.AddIPToCacheAsync(ip, ipAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating cache for IP {ip}: {ex.Message}");
                throw;
            }
        }
    }
}
