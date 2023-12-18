using Microsoft.AspNetCore.Identity;
using multiTenantManagement.Models.Common;

namespace multiTenantManagement.Models.Identities
{
    public class UserLogin : IdentityUserLogin<string> , IAuditableEntity, IHaveTenant
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string TenantId { get; set; }
    }
}
