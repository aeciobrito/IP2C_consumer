using IP2C_consumer.Database;
using IP2C_consumer.Models;
using IP2C_consumer.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IP2C_consumer.Services
{
    public class IPReportService : IIPReportService
    {
        public async Task<List<CountryReportItem>> GenerateCountryAddressReport(AppDbContext dbContext, string[] countryCodes)
        {
            string sqlQuery = @"
                SELECT 
                    C.Name AS Name, 
                    COUNT(I.Id) AS AddressesCount, 
                    MAX(I.UpdatedAt) AS LastAddressUpdated
                FROM Countries C
                LEFT JOIN IPAddresses I ON C.Id = I.CountryId
                {WHERE_CONDITION}
                GROUP BY C.Name";

            string whereCondition = "";
            var parameters = new List<SqlParameter>();

            if (countryCodes != null && countryCodes.Length > 0)
            {
                whereCondition = "WHERE C.TwoLetterCode IN (" +
                                 string.Join(", ", countryCodes.Select((_, i) => $"@CountryCode{i}")) + ")";
                for (int i = 0; i < countryCodes.Length; i++)
                {
                    parameters.Add(new SqlParameter($"@CountryCode{i}", countryCodes[i]));
                }
            }

            sqlQuery = sqlQuery.Replace("{WHERE_CONDITION}", whereCondition);

            var reportItems = await dbContext
                .Database
                .SqlQueryRaw<CountryReportItem>(sqlQuery, parameters.ToArray())
                .ToListAsync();

            return reportItems;
        }
    }
}
