﻿namespace multiTenantManagement.Models.Dtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }

    }
}
