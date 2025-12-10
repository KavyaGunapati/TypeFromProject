using System.ComponentModel.DataAnnotations;

namespace TypeFormProject.DataAccess.Entities
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserOrgRole> UserRoles { get; set; } = new List<UserOrgRole>();
    }
}
