using JobApplication.Domain.Enums;

namespace JobApplication.Application.Interfaces
{
    public interface ICurrentUser
    {
        int UserId { get; }
        UserRole Role { get; }
    }
}
