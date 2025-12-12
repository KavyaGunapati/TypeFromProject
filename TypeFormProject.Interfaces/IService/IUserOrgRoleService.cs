using DTO= TypeFormProject.Models.DTOs;
namespace TypeFormProject.Interfaces.IService
{
    public interface IUserOrgRoleService
    {
        Task<DTO.Result<DTO.UserOrgRole>> AssignAsync(DTO.AssignUserOrgRole dto);
        Task<DTO.Result<List<DTO.UserOrgRole>>> GetByOrganizationAsync(int organizationId);
        Task<DTO.Result> RemoveAsync(int organizationId, string userId);
        Task<DTO.Result<DTO.UserOrgRole>> ChangeRoleAsync(int organizationId, string userId, int newRole);
    }
}
