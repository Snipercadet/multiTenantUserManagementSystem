using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using multiTenantManagement.Data.EntityConfiguration;
using multiTenantManagement.Models.Common;
using multiTenantManagement.Models.Identities;
using multiTenantManagement.Services.Contract;

namespace multiTenantManagement.Data.Contexts
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole,string,UserClaim,UserRole,UserLogin,RoleClaim,UserToken>
    {
        private readonly ICurrentTenantService _tenantService;
        public string CurrentTenantId { get; set; }
        public string CurrentTenantTConnectionString { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantService tenantService) : base(options)
        {
            _tenantService = tenantService;
            CurrentTenantId = _tenantService.TenantId;
            CurrentTenantTConnectionString = _tenantService.ConnectionString;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        entry.Entity.LastModifiedAt = DateTime.Now;
                        break;
                    default:
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IHaveTenant>())
            {
                switch (entry.State)
                {
                    
                    case EntityState.Added:
                        entry.Entity.TenantId = CurrentTenantId;
                        break;
                    default:
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            IdentityUserConfiguration.ApplyUserConfiguration(modelBuilder, CurrentTenantId);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityUserConfiguration).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.Development.json")
                .Build();

            var conn = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(conn);

            string tenantConnectionString = CurrentTenantTConnectionString;

            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                _ = optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }
    }
}
