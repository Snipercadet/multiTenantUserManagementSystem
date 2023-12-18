using Microsoft.AspNetCore.Authorization;

namespace multiTenantManagement.PolicyAuthorization
{
    public class PolicyRequirement : IAuthorizationRequirement
    {
        public string Status { get; set; }
        public PolicyRequirement(string status)
        {
            Status = status;
        }
    }
}
