using IP2C_consumer.Database;
using IP2C_consumer.Models;
using IP2C_consumer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IP2C_consumer.Services
{
    public class IPUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private const int BatchSize = 100;
        private const int DelayInHours = 1;

        public IPUpdaterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessIPAddresses(stoppingToken);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Critical error in IPUpdaterService: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(DelayInHours), stoppingToken);
            }
        }

        private async Task ProcessIPAddresses(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ip2cService = scope.ServiceProvider.GetRequiredService<IIP2CService>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            int skip = 0;
            bool hasMoreRecords;

            do
            {
                List<string> ipsToProcess = await GetIpsInBatch(dbContext, BatchSize, skip);
                if (ipsToProcess.Count == 0) break; 

                foreach (var ipAddress in ipsToProcess)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var updatedCountry = await ip2cService.FetchCountryFromIPAsync(ipAddress);
                    if (updatedCountry != null && HasCountryChanged(dbContext, ipAddress, updatedCountry))
                    {
                        var existingIpEntry = dbContext.IPAddresses.FirstOrDefault(i => i.IP == ipAddress);

                        if (existingIpEntry != null)
                        {
                            existingIpEntry.CountryId = updatedCountry.Id;
                            existingIpEntry.UpdatedAt = DateTime.UtcNow;

                            await cacheService.CacheCountryAsync(ipAddress, updatedCountry);
                        }
                    }
                }

                skip += BatchSize;

            } while ((hasMoreRecords = await HasMoreIPs(dbContext, skip)));

            await dbContext.SaveChangesAsync();
        }

        private async Task<bool> HasMoreIPs(AppDbContext context, int skip)
            => (await context.IPAddresses.CountAsync()) > (skip + BatchSize);

        private async Task<List<string>> GetIpsInBatch(AppDbContext context, int batchSize, int skip)
        {
            var ipAddresses = await context.IPAddresses
                .Select(ip => ip.IP)
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync();

            return ipAddresses;
        }

        private bool HasCountryChanged(AppDbContext dbContext, string ipAddress, Country updatedCountry)
        {
            var existingIpEntry = dbContext.IPAddresses.Include(i => i.Country).FirstOrDefault(i => i.IP == ipAddress);

            if (existingIpEntry == null) return true;

            var existingCountry = existingIpEntry?.Country;

            if (existingCountry == null)
                return true;

            return
                updatedCountry.TwoLetterCode != existingCountry.TwoLetterCode ||
                updatedCountry.ThreeLetterCode != existingCountry.ThreeLetterCode ||
                updatedCountry.Name != existingCountry.Name;
        }
    }
}
