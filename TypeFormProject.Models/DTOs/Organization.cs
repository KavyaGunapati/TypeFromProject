using System.ComponentModel.DataAnnotations;

namespace TypeFormProject.Models.DTOs
{

    public record OrganizationDto(int Id, string Name, DateTime CreatedAt);

    public record CreateOrganizationRequest(
        [Required, MaxLength(150)] string Name
    );

    public record UpdateOrganizationNameRequest(
        [Required] int OrganizationId,
        [Required, MaxLength(150)] string NewName
    );

    public record OrganizationMemberDto(
        int UserId,
        string Email,
        string DisplayName,
        string Role 
    );
}



