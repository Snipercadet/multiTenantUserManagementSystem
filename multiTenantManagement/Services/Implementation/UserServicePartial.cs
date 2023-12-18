using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using multiTenantManagement.Models.Dtos;
using multiTenantManagement.Models.Identities;
using multiTenantManagement.Utilities;
using System.Net;
using System.Security.Claims;

namespace multiTenantManagement.Services.Implementation
{
    public partial class UserService
    {
        public async Task<BaseResponse<bool>> AddNewRole(string roleName)
        {
            var role = await _roleManager.RoleExistsAsync(roleName);
            if (role)
            {
                return new BaseResponse<bool>("Role name already exist", HttpStatusCode.BadRequest);
            }

            var newRole = new AppRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
            };

            await _roleManager.CreateAsync(newRole);

            return new BaseResponse<bool>(true,HttpStatusCode.OK);
        }

        public async Task<BaseResponse<List<RolesResponseDto>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            if (roles is null || roles.Count == 0)
            {
                return new BaseResponse<List<RolesResponseDto>>("No roles found", HttpStatusCode.NotFound);
            }

            var rolesDto = new List<RolesResponseDto>();
            foreach (var role in roles)
            {
                rolesDto.Add(new RolesResponseDto
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return new BaseResponse<List<RolesResponseDto>>(rolesDto, HttpStatusCode.OK);
        }

        public async Task<BaseResponse<List<Claim>>> GetRolePermissions(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var roleclaims = await _roleManager.GetClaimsAsync(role);

            var permissionClaims = roleclaims.Where(x => x.Type == "Permissions").ToList();

            return new BaseResponse<List<Claim>>("Role added successfully", HttpStatusCode.OK);
        }

        public async Task<BaseResponse<RolesDto>> GetRoleByIdOrName(string nameOrId)
        {
            if (string.IsNullOrEmpty(nameOrId))
            {
                throw new ArgumentNullException(nameof(nameOrId));
            }
            var role = await _roleManager.FindByIdAsync(nameOrId) ?? await _roleManager.FindByNameAsync(nameOrId);
            if (role is null)
            {
                return new BaseResponse<RolesDto>("Role does not exist",HttpStatusCode.NotFound);
            }
            var roleDto = new RolesDto
            {
                Id = role.Id,
                Name = role.Name
            };

            return new BaseResponse<RolesDto>(roleDto,HttpStatusCode.NotFound);
        }

        //public async Task<BaseResponse<bool>> DeleteRole(string nameOrId)
        //{
        //    if (string.IsNullOrEmpty(nameOrId))
        //    {
        //        throw new ArgumentNullException(nameof(nameOrId));
        //    }
        //    var role = await _roleManager.FindByIdAsync(nameOrId) ?? await _roleManager.FindByNameAsync(nameOrId);
        //    if (role is null)
        //    {
        //        throw new RestException(HttpStatusCode.NotFound, "Role not found");
        //    }

        //    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        //    if (usersInRole.Count > 0)
        //    {
        //        throw new RestException(HttpStatusCode.NotFound, "Role is currently in use");
        //    }

        //    await _roleManager.DeleteAsync(role);
        //    return new BaseResponse<bool>
        //    {
        //        Data = true,
        //        Message = "Role deleted successfully"
        //    };
        //}

        public async Task<BaseResponse<bool>> AddUserToRole(AddUserToRoleRequest request)
        {
            var user =
                await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
            {
                return new BaseResponse<bool>("User not found", HttpStatusCode.NotFound);
            }

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null)
            {
                return new BaseResponse<bool>("Role not found", HttpStatusCode.NotFound);
            }

            var userRoleChk = await _userManager.IsInRoleAsync(user, role.Name);
            if (userRoleChk)
            {
                return new BaseResponse<bool>("User already added to role", HttpStatusCode.NotFound);
            }

            var addUsertoRole = await _userManager.AddToRoleAsync(user, role.Name);
            if (addUsertoRole.Succeeded)
            {
                return new BaseResponse<bool>(true, HttpStatusCode.OK);
            }
            return new BaseResponse<bool>(false, HttpStatusCode.InternalServerError);
        }

        //public async Task<BaseResponse<bool>> RemoveUserFromRole(RemoveUserFromRoleRequest model)
        //{
        //    var user = await _userManager.FindByIdAsync(model.UserId) ?? throw new RestException(HttpStatusCode.NotFound, "user not found");
        //    var role = await _roleManager.FindByIdAsync(model.RoleId) ?? throw new RestException(HttpStatusCode.NotFound, "role not found");

        //    var userInRole = await _userManager.IsInRoleAsync(user, role.Name);
        //    if (!userInRole)
        //    {
        //        throw new RestException(HttpStatusCode.BadRequest, "user does not have this role");
        //    }

        //    await _userManager.RemoveFromRoleAsync(user, role.Name);
        //    return new BaseResponse<bool>
        //    {
        //        Data = true,
        //        Message = "user removed from role successfully"
        //    };
        //}

        public async Task<BaseResponse<bool>> AddRolePermissions(AddPermissionToRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            var claims = new List<Claim>();
            if (role is null) return new BaseResponse<bool>("role does not exist", HttpStatusCode.NotFound);
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var permission in request.Permissions)
            {
                if (roleClaims.Any(x => x.Value == permission))
                {
                    return new BaseResponse<bool>( $"{role.Name} already has this permission", HttpStatusCode.BadRequest);
                };

                var newClaim = new Claim("Permission", permission);
                claims.Add(newClaim);
            }

            foreach (var claim in claims)
            {
                var result = await _roleManager.AddClaimAsync(role, claim);

                if (!result.Succeeded)
                {
                    return new BaseResponse<bool>( "Failed to add role claim", HttpStatusCode.InternalServerError);
                }
            }
            return new BaseResponse<bool>(true, HttpStatusCode.OK);
        }

        //public async Task<BaseResponse<bool>> RemoveRolePermission(RemovePermissionFromRoleRequest model)
        //{
        //    var result = new IdentityResult();
        //    var role = await _roleManager.FindByIdAsync(model.RoleId) ?? throw new RestException(HttpStatusCode.NotFound, "role not found");

        //    if (model == null)
        //    {
        //        throw new RestException(HttpStatusCode.BadGateway, "please enter valid inputs for permissions");
        //    }

        //    var roleClaim = await _roleManager.GetClaimsAsync(role);
        //    foreach (var permission in model.Permissions)
        //    {
        //        var matchingClaim = roleClaim.FirstOrDefault(x => x.Type == "Permissions" && x.Value == permission);

        //        if (matchingClaim != null)
        //        {
        //            result = await _roleManager.RemoveClaimAsync(role, matchingClaim);
        //            if (!result.Succeeded)
        //            {
        //                throw new RestException(HttpStatusCode.InternalServerError, "Failed to remove permission");
        //            }
        //        }
        //    }

        //    return new BaseResponse<bool>
        //    {
        //        Data = true,
        //        Message = "permissions removed successfully"
        //    };
        //}

    }
}
