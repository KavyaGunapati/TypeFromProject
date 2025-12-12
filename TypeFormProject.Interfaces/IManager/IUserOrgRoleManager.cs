
using TypeFormProject.Models.DTOs;

namespace TypeFormProject.Interfaces.IManager
{
    public interface IUserOrgRoleManager
    {
        Task<Result<UserOrgRole>> AssignAsync(AssignUserOrgRole dto);
        Task<Result<List<UserOrgRole>>> GetByOrganizationAsync(int organizationId);
        Task<Result> RemoveAsync(int organizationId, string userId);
        Task<Result<UserOrgRole>> ChangeRoleAsync(int organizationId, string userId, int newRole);
    }
}

