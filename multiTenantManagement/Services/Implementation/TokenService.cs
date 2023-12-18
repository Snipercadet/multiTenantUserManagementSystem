using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using multiTenantManagement.Models.Dtos;
using multiTenantManagement.Models.Identities;
using multiTenantManagement.Services.Contract;
using multiTenantManagement.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace multiTenantManagement.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }


        public TokenResponseDto GenerateToken(AppUser user, IList<string> roles, List<Claim> permissions)
        {

            var claims = GetUserClaims(user,roles,permissions);
            var securityToken = CreateSecurityToken(claims);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            var response = new TokenResponseDto
            {
                Token = token,
                Expires = DateTime.Now.AddHours(60)
            };
            return response;
        }

        private JwtSecurityToken CreateSecurityToken(List<Claim> claims)
        {
            var signingCredentials = GenerateSigningCredentials();
            var securityToken = new JwtSecurityToken
                (issuer: _jwtSettings.ValidIssuer, audience: _jwtSettings.ValidAudience, claims:claims,expires: DateTime.Now.AddHours(60), signingCredentials: signingCredentials);
            //var jwtPayload = new JwtPayload(issuer: _jwtSettings.ValidIssuer, audience: _jwtSettings.ValidAudience, claims: claims, notBefore:null,expires: DateTime.UtcNow.AddSeconds(_jwtSettings.TokenExpiration));
            //var header =  new JwtHeader(signingCredentials);
            //var serializedHeader = JsonConvert.SerializeObject(header);
            //var serializePayload = JsonConvert.SerializeObject(jwtPayload);
            //var jsonWebToken = new JsonWebToken(serializedHeader,serializePayload);
            //var writeToken = new JsonWebTokenHandler().CreateToken(serializePayload, signingCredentials);

            return securityToken;
        }

        private SigningCredentials GenerateSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            return signingCredentials;
        }

        private List<Claim> GetUserClaims(AppUser user, IList<string> roles, List<Claim> permissions)
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypeHelper.Email, user.Email),
                new(ClaimTypeHelper.TenantId, user.TenantId),
                new("expires",$"{DateTime.Now.AddHours(60)}"),

            };

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypeHelper.Role, role));
            }

            claims.AddRange(roleClaims);
            claims.AddRange(permissions);
            return claims;
        }
    }
}
