using IP2C_consumer.Database;
using IP2C_consumer.Models;
using IP2C_consumer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IP2C_consumer.Services
{
    public class IPDetailsService : IIPDetailsService
    {
        private readonly ICacheService _cacheService;
        private readonly IIP2CService _ip2cService;
        private readonly AppDbContext _dbContext;

        public IPDetailsService(ICacheService cacheService, IIP2CService ip2cService, AppDbContext dbContext)
        {
            _cacheService = cacheService;
            _ip2cService = ip2cService;
            _dbContext = dbContext;
        }

        public async Task<Country> GetIPDetailsAsync(string ipAddress)
        {
            // 1. Check Cache
            var country = await _cacheService.GetCachedCountryAsync(ipAddress);
            if (country != null) return country;

            // 2. Check Database
            country = await _dbContext.IPAddresses
                .Where(ip => ip.IP == ipAddress)
                .Select(ip => ip.Country)
                .FirstOrDefaultAsync();

            if (country != null)
            {
                await _cacheService.CacheCountryAsync(ipAddress, country);
                return country;
            }

            // 3. Fallback to IP2C
            country = await _ip2cService.FetchCountryFromIPAsync(ipAddress);
            if (country != null)
            {
                // Persist in Database
                var dbCountry = await _dbContext.Countries
                    .FirstOrDefaultAsync(c => c.ThreeLetterCode == country.ThreeLetterCode);

                if (dbCountry == null)
                {
                    dbCountry = new Country
                    {
                        Name = country.Name,
                        TwoLetterCode = country.TwoLetterCode,
                        ThreeLetterCode = country.ThreeLetterCode,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.Countries.Add(dbCountry);
                    await _dbContext.SaveChangesAsync();
                }

                var ipAddressEntry = new IpAddress
                {
                    IP = ipAddress,
                    CountryId = dbCountry.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _dbContext.IPAddresses.Add(ipAddressEntry);
                await _dbContext.SaveChangesAsync();

                // Cache the result
                await _cacheService.CacheCountryAsync(ipAddress, dbCountry);
            }

            return country;
        }

        public bool IsValidIpAddress(string ipAddress)
        {
            return IpAddressValidator.IsValidIpAddress(ipAddress);
        }
    }
}
