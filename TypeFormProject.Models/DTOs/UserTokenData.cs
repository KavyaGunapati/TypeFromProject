namespace TypeFormProject.Models.DTOs
{
        public class UserTokenData
        {
            public string UserId { get; set; } = default!;
            public string UserName { get; set; } = default!;
            public string? Email { get; set; }
            public IList<string> Roles { get; set; } = new List<string>();
        }
}
    
