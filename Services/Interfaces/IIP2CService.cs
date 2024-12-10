using IP2C_consumer.Models;

namespace IP2C_consumer.Services.Interfaces
{
    public interface IIP2CService
    {
        Task<Country> FetchCountryFromIPAsync(string ipAddress);
    }
}
