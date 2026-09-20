using JobApplication.Domain.Enums;
using System;

namespace JobApplication.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public int? CandidateId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
