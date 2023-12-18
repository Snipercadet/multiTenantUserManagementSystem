using multiTenantManagement.Models.Dtos;
using multiTenantManagement.Utilities;
using System.Security.Claims;

namespace multiTenantManagement.Services.Contract
{
    public interface IUserService
    {
        Task<BaseResponse<bool>> Registration(UserRegistrationRequestDto model);
        Task<BaseResponse<UserLoginResponse>> Login(UserLoginRequest model);
        Task<BaseResponse<bool>> AddNewRole(string roleName);
        Task<BaseResponse<List<RolesResponseDto>>> GetRoles();
        Task<BaseResponse<List<Claim>>> GetRolePermissions(string roleName);
        Task<BaseResponse<RolesDto>> GetRoleByIdOrName(string nameOrId);
        Task<BaseResponse<bool>> AddUserToRole(AddUserToRoleRequest request);
        Task<BaseResponse<bool>> AddRolePermissions(AddPermissionToRoleRequest request);
        Task<BaseResponse<List<RolesResponseDto>>> GetAllUsers();
 
    }
}
