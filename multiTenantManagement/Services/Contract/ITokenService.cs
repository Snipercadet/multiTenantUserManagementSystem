using multiTenantManagement.Models.Dtos;
using multiTenantManagement.Models.Identities;
using System.Security.Claims;

namespace multiTenantManagement.Services.Contract
{
    public interface ITokenService
    {
        TokenResponseDto GenerateToken(AppUser user, IList<string> roles, List<Claim> permissions);
    }
}
