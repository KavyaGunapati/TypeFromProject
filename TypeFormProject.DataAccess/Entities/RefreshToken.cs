namespace TypeFormProject.DataAccess.Entities
{

    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;
        public string UserId { get; set; } = default!; 
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public AppUser User { get; set; } = default!;
    }

}
