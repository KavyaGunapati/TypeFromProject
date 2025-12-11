
using System.ComponentModel.DataAnnotations;

namespace TypeFormProject.Models.DTOs
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateOrganization
    {
        [Required]
        public string Name { get; set; } = null!;
    }

    public class UpdateOrganization
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public bool? IsDeleted { get; set; } 
    }
}




