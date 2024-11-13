using api.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace api.Infraestructure
{
    public class IP2CCacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDistributedCache _cache;

        public IP2CCacheService(IConnectionMultiplexer redisConnection, IDistributedCache cache)
        {
            _redisConnection = redisConnection;
            _cache = cache;
        }

        public async Task<IPAddress> GetIPFromCacheAsync(string ip)
        {
            var cacheKey = $"IP2C:{ip}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {                
                return JsonConvert.DeserializeObject<IPAddress>(cachedData);
            }
            
            return null;
        }

        public async Task AddIPToCacheAsync(string ip, IPAddress ipAddress)
        {
            var cacheKey = $"IP2C:{ip}";

            if (ipAddress != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Configura o tempo de expiração do cache
                };

                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(ipAddress), options);
            }
        }

        public async Task RemoveIPFromCacheAsync(string ip)
        {
            var cacheKey = $"IP2C:{ip}";

            await _cache.RemoveAsync(cacheKey);
        }
    }
}
