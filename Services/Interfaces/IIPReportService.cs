using IP2C_consumer.Database;
using IP2C_consumer.Models;

namespace IP2C_consumer.Services.Interfaces
{
    public interface IIPReportService
    {
        Task<List<CountryReportItem>> GenerateCountryAddressReport(AppDbContext dbContext, string[] countryCodes);
    }
}
