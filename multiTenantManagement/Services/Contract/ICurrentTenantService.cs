namespace multiTenantManagement.Services.Contract
{
    public interface ICurrentTenantService
    {
        string TenantId { get; set; }
        string ConnectionString { get; set; }
        Task<bool> SetTenant(string tenant);
    }
}
