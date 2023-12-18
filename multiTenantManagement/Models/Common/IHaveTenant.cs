namespace multiTenantManagement.Models.Common
{
    public interface IHaveTenant
    {
        string TenantId { get; set; }
    }
}
