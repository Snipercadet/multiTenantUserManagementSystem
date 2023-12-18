using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace multiTenantManagement.PolicyAuthorization
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);
            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PolicyRequirement(policyName))
                    .Build();

                _options.AddPolicy(policyName, policy);
            }
            return policy;
        }
    }
}
