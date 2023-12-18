namespace multiTenantManagement.Models.Dtos
{
    public class TenantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string SubscriptionLevel { get; set; }
    }


}
