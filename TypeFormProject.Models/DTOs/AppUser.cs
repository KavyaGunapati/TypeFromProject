using System.ComponentModel.DataAnnotations;

namespace TypeFormProject.Models.DTOs
{
    public class Register
    {
        [Required, StringLength(50)]
        public string FullName { get; set; } = null!;
        [Required]
        public long PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string PasswordHash { get; set; } = null!;
    }
    public class Login
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string PasswordHash { get; set; } = null!;
    }


}
