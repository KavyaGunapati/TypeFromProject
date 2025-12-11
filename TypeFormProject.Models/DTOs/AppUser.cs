using System.ComponentModel.DataAnnotations;

namespace TypeFormProject.Models.DTOs
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Phone]
        public string? Phone { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
    public class Login
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
    public class AuthResponse
    {
           public string AccessToken { get; set; } = default!;
            public string RefreshToken { get; set; } = default!;
            public DateTime AccessTokenExpiresAt { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = default!;
    }


}
