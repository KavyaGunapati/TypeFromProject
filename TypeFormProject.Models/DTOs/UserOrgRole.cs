namespace TypeFormProject.Models.DTOs
{
    public class UserOrgRoleDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string UserId { get; set; } = default!;
        public int Role { get; set; }
    }

    public class AssignUserOrgRoleDto
    {
        public int OrganizationId { get; set; }
        public string UserId { get; set; } = default!;
        public int Role { get; set; }
    }
}
