using Microsoft.EntityFrameworkCore;
using multiTenantManagement.Data.Contexts;
using multiTenantManagement.Services.Contract;

namespace multiTenantManagement.Services.Implementation
{
    public class CurrentTenantService : ICurrentTenantService
    {
        private readonly TenantDbContext _tenantDbContext;
        public CurrentTenantService(TenantDbContext tenantDbContext)
        {
            _tenantDbContext = tenantDbContext;
        }

        public string TenantId { get; set; }
        public string ConnectionString { get; set; }

        public async Task<bool> SetTenant(string tenant)
        {
            var tenantInfo = await _tenantDbContext.Tenants.Where(x => x.Id == tenant).FirstOrDefaultAsync();
            if (tenantInfo != null)
            {
                TenantId = tenantInfo.Id;
                ConnectionString = tenantInfo.ConnectionString;
                return true;
            }
            else
            {
                return false;
            }
        }
        //public string TenantId { get ; set ; }
        //public string ConnectionString { get ; set ; }

        //public Task<bool> SetTenant(string tenant)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
