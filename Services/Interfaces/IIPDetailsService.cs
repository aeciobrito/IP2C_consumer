using IP2C_consumer.Models;

namespace IP2C_consumer.Services.Interfaces
{
    public interface IIPDetailsService
    {
        Task<Country> GetIPDetailsAsync(string ipAddress);
        bool IsValidIpAddress(string ipAddress);
    }
}
