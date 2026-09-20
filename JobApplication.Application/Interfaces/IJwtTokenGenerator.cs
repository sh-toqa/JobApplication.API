using JobApplication.Domain.Entities;
using System;

namespace JobApplication.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
    }
}
