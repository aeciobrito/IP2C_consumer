using IP2C_consumer.Database;

namespace IP2C_consumer.Services.Interfaces
{
    public interface ICachedIpUpdater
    {
        Task UpdateAllIPs(AppDbContext dbContext);
    }
}
