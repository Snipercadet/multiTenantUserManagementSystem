using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using multiTenantManagement.Models.Dtos;
using multiTenantManagement.Services.Contract;
using multiTenantManagement.Utilities;

namespace multiTenantManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> Register(UserRegistrationRequestDto model)
        {

            var result = await _userService.Registration(model);
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserLoginResponse), 200)]
        public async Task<IActionResult> Login(UserLoginRequest model)
        {
            var result = await _userService.Login(model);
            return Ok(result);
        }

        [HttpGet("users")]
        [Authorize("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {

            var result = await _userService.GetAllUsers();
            return Ok(result);
        }
        [HttpPost("add-role")]
        //[ProducesResponseType(typeof(RegistrationDto), 200)]
        public async Task<IActionResult> AddNewRole(string roleName)
        {
            var result = await _userService.AddNewRole(roleName);
            return Ok(result);
        }

        [HttpPost("add-user-to-role")]
        //[ProducesResponseType(typeof(RegistrationDto), 200)]
        public async Task<IActionResult> AddUserToRole(AddUserToRoleRequest model)
        {
            var result = await _userService.AddUserToRole(model);
            return Ok(result);
        }

        [HttpPost("add-role-permission")]
        //[ProducesResponseType(typeof(RegistrationDto), 200)]
        public async Task<IActionResult> AddRolePermissions(AddPermissionToRoleRequest model)
        {
            var result = await _userService.AddRolePermissions(model);
            return Ok(result);
        }

        [HttpGet("roles")]
        [Authorize("GetAll")]
        //[ProducesResponseType(typeof(RegistrationDto), 200)]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _userService.GetRoles();
            return Ok(result);
        }
    }
}
