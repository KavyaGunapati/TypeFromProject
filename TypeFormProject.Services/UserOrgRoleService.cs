using Microsoft.Extensions.Logging;
using TypeFormProject.Interfaces.IManager;
using TypeFormProject.Interfaces.IService;
using TypeFormProject.Models.DTOs;

namespace TypeFormProject.Services
{
    public class UserOrgRoleService : IUserOrgRoleService
    {
        private readonly IUserOrgRoleManager _manager;
        private readonly ILogger<UserOrgRoleService> _logger;
        public UserOrgRoleService(IUserOrgRoleManager manager, ILogger<UserOrgRoleService> logger)
        {
            _manager = manager;
            _logger = logger;
        }
        public async Task<Result<UserOrgRole>> AssignAsync(AssignUserOrgRole dto)
        {
            try
            {
                var result = await _manager.AssignAsync(dto);
                _logger.LogInformation("Assigned role {Role} to user {UserId} in organization {OrganizationId}", dto.Role, dto.UserId, dto.OrganizationId);
                return new Result<UserOrgRole> { Success=result.Success,Message=result.Message,Data=result.Data};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {Role} to user {UserId} in organization {OrganizationId}", dto.Role, dto.UserId, dto.OrganizationId);
                return new Result<UserOrgRole>
                {
                    Success = false,
                    Message = "An error occurred while assigning the role."
                };
            }
        }

        public async Task<Result<UserOrgRole>> ChangeRoleAsync(int organizationId, string userId, int newRole)
        {
            try
            {
                var result = await _manager.ChangeRoleAsync(organizationId, userId, newRole);
                _logger.LogInformation("Changed role to {NewRole} for user {UserId} in organization {OrganizationId}", newRole, userId, organizationId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role to {NewRole} for user {UserId} in organization {OrganizationId}", newRole, userId, organizationId);
                return new Result<UserOrgRole>
                {
                    Success = false,
                    Message = "An error occurred while changing the role."
                };
            }
        }

        public async Task<Result<List<UserOrgRole>>> GetByOrganizationAsync(int organizationId)
        {
            try
            {
                var result = await _manager.GetByOrganizationAsync(organizationId);
                _logger.LogInformation("Retrieved roles for organization {OrganizationId}", organizationId);
                return new Result<List<UserOrgRole>> { Success = result.Success, Message = result.Message, Data = result.Data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for organization {OrganizationId}", organizationId);
                return new Result<List<UserOrgRole>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving roles."
                };
            }
        }

        public async Task<Result> RemoveAsync(int organizationId, string userId)
        {
            try
            {
                var result = await _manager.RemoveAsync(organizationId, userId);
                _logger.LogInformation("Removed user {UserId} from organization {OrganizationId}", userId, organizationId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from organization {OrganizationId}", userId, organizationId);
                return new Result
                {
                    Success = false,
                    Message = "An error occurred while removing the user."
                };
            }
        }
    }
}
