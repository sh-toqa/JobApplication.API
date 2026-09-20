using JobApplication.Application.Interfaces;
using JobApplication.Domain.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JobApplication.API.Services
{
    public class CurrentUser : ICurrentUser
    {
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            var subClaim = user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (subClaim is not null && int.TryParse(subClaim, out var userId))
            {
                UserId = userId;
            }

            var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim is not null && Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                Role = role;
            }
        }

        public int UserId { get; }
        public UserRole Role { get; }
    }
}
