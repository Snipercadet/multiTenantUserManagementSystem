namespace multiTenantManagement.Models.Dtos
{
    public class UserRegistrationRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserRegistrationResponseDto
    {
        public string message { get; set; }
    }

    public class UserLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginResponse
    {
        public string Status { get; set; }
        public TokenResponseDto Token { get; set; }
    }
}
