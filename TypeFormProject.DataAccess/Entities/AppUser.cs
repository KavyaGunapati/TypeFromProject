using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TypeFormProject.DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public ICollection<UserOrgRole> OrgRoles { get; set; } = new List<UserOrgRole>();
    }

}
