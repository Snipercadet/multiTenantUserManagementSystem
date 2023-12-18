using Microsoft.AspNetCore.Identity;
using multiTenantManagement.Models.Common;

namespace multiTenantManagement.Models.Identities
{
    public class AppUser : IdentityUser<string> , IAuditableEntity , IHaveTenant
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string TenantId { get; set; }
    }
}
