using IP2C_consumer.Models;

namespace IP2C_consumer.Services.Interfaces
{
    public interface ICacheService
    {
        Task<Country> GetCachedCountryAsync(string ipAddress);
        Task CacheCountryAsync(string ipAddress, Country country);
    }
}
