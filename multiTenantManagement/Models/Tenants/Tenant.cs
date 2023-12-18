using multiTenantManagement.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace multiTenantManagement.Models.Tenants
{
    public class Tenant : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string SubscriptionLevel { get; set; }
    }
}
