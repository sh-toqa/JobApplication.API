using JobApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobApplication.Application.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public string? Name { get; set; }

        public string? CvUrl { get; set; }
    }
}
