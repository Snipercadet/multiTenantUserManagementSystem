using Microsoft.EntityFrameworkCore;
using multiTenantManagement.Models.Tenants;

namespace multiTenantManagement.Data.Contexts
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
            
        }

        public TenantDbContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true)
                .Build();

            var conn = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(conn);
        }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
