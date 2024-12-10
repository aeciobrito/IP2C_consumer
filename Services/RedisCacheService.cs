using IP2C_consumer.Models;
using IP2C_consumer.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace IP2C_consumer.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private const string CacheKeyPrefix = "IP_";

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Country> GetCachedCountryAsync(string ipAddress)
        {
            var cachedData = await _database.StringGetAsync($"{CacheKeyPrefix}{ipAddress}");
            return cachedData.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Country>(cachedData);
        }

        public async Task CacheCountryAsync(string ipAddress, Country country)
        {
            var serializedData = JsonSerializer.Serialize(country);
            await _database.StringSetAsync($"{CacheKeyPrefix}{ipAddress}", serializedData, TimeSpan.FromHours(1));
        }
    }
}
