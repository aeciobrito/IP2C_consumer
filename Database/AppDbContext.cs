using IP2C_consumer.Models;
using Microsoft.EntityFrameworkCore;

namespace IP2C_consumer.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<IpAddress> IPAddresses { get; set; }
    }
}
