namespace multiTenantManagement.Models.Dtos
{
    public class RolesDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RolesResponseDto : RolesDto
    {
    }

    public class AddUserToRoleRequest
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

    public class RemoveUserFromRoleRequest : AddUserToRoleRequest
    {

    }

    public class AddPermissionToRoleRequest
    {
        public string RoleId { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class RemovePermissionFromRoleRequest : AddPermissionToRoleRequest
    {
    }
}
