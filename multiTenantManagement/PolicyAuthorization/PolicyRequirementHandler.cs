using Microsoft.AspNetCore.Authorization;

namespace multiTenantManagement.PolicyAuthorization
{
    public class PolicyRequirementHandler : AuthorizationHandler<PolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            var permissionClaim = context.User.Claims.Where(x => x.Type.ToLower() == "permission").ToList();
            if (!permissionClaim.Any() || !permissionClaim.Any(x => x.Value == requirement.Status))
            {
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
