using System.ComponentModel.DataAnnotations;
using TypeFormProject.DataAccess.Enums;

namespace TypeFormProject.DataAccess.Entities
{
    public class UserOrgRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public OrgRole Role { get; set; }

        public Organization Organization { get; set; } = default!;
        public AppUser User { get; set; } = default!;
    }
}
