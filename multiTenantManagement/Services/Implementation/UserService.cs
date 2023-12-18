using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using multiTenantManagement.Data.Contexts;
using multiTenantManagement.Models.Dtos;
using multiTenantManagement.Models.Enums;
using multiTenantManagement.Models.Identities;
using multiTenantManagement.Models.Tenants;
using multiTenantManagement.Services.Contract;
using multiTenantManagement.Utilities;
using System.Net;
using System.Security.Claims;

namespace multiTenantManagement.Services.Implementation
{
    public partial class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly TenantDbContext _tenantContext;
        private readonly AppDbContext _appContext;

        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, ITokenService tokenService, AppDbContext appContext, TenantDbContext tenantContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            //_tenantContext = tenantContext;
            _appContext = appContext;
            _tenantContext = tenantContext;
        }

        public async Task<BaseResponse<bool>> Registration(UserRegistrationRequestDto model)
        {
            var userCheck = await _userManager.Users.AnyAsync(x => x.Email.Trim().ToLower() == model.Email.Trim().ToLower());
            if (userCheck)
            {
                return new BaseResponse<bool>("Email already exist", HttpStatusCode.BadRequest);
            }

            var newTenant = new Tenant
            {
                Id = model.CompanyName,
                Name = model.CompanyName,
                SubscriptionLevel = "Mini"
            };

            await _tenantContext.Tenants.AddAsync(newTenant);
            await _tenantContext.SaveChangesAsync();

            var newUser = new AppUser
             {
                 Id = Guid.NewGuid().ToString(),
                 FirstName = model.FirstName,
                 LastName = model.LastName,
                 UserName = model.Username,
                 Email = model.Email,
                 TenantId = newTenant.Id
             };

         
            var userCraetionResponse = await _userManager.CreateAsync(newUser, model.Password);
            if (userCraetionResponse.Succeeded)
            {
                var isInRole = await _userManager.IsInRoleAsync(newUser, ERole.TenantSuperAdmin.ToString());
                if (!isInRole)
                {
                    await _userManager.AddToRoleAsync(newUser, ERole.TenantSuperAdmin.ToString());
                    return new BaseResponse<bool>(true, HttpStatusCode.OK);
                }
            }

            return new BaseResponse<bool>("something went wrong", HttpStatusCode.InternalServerError, userCraetionResponse.Errors);
        }

        public async Task<BaseResponse<UserLoginResponse>> Login(UserLoginRequest model)
        {
            //var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
            //var xx = await _userManager.FindByEmailAsync(model.Username) ?? await _userManager.FindByNameAsync(model.Username);

            var user = await _appContext.Users.Where(x => x.UserName == model.Username).IgnoreQueryFilters().FirstOrDefaultAsync() ?? await _appContext.Users.Where(x => x.Email == model.Username).IgnoreQueryFilters().FirstOrDefaultAsync();
            if (user != null)
            {
                //var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Username);
                //if (user == null)
                //{
                //    return new BaseResponse<UserLoginResponse>("Invalid login detaila", HttpStatusCode.BadRequest);
                //}

                var roles = await _userManager.GetRolesAsync(user);
                var rolePermissionClaim = new List<Claim>();
                foreach (var role in roles)
                {
                    var getRole = await _roleManager.FindByNameAsync(role);
                    if (getRole == null) { return new BaseResponse<UserLoginResponse>("Role not found for user", HttpStatusCode.NotFound); };
                    //get claims for roles
                    var claims = await _roleManager.GetClaimsAsync(getRole);
                    rolePermissionClaim.AddRange(claims);
                }

                var token = _tokenService.GenerateToken(user, roles, rolePermissionClaim);
                var response = new UserLoginResponse
                {
                    Status = EUserStatus.Active.ToString(),
                    Token = token
                };
                return new BaseResponse<UserLoginResponse>(response, HttpStatusCode.OK);
            }


            return new BaseResponse<UserLoginResponse>("", HttpStatusCode.BadRequest);
        }

        public async Task<BaseResponse<List<RolesResponseDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users is null || users.Count == 0)
            {
                return new BaseResponse<List<RolesResponseDto>>("No users found", HttpStatusCode.NotFound);
            }

            var rolesDto = new List<RolesResponseDto>();
            foreach (var user in users)
            {
                rolesDto.Add(new RolesResponseDto
                {
                    Id = user.Id,
                    Name = user.UserName
                });
            }

            return new BaseResponse<List<RolesResponseDto>>(rolesDto, HttpStatusCode.OK);
        }
    }
}
