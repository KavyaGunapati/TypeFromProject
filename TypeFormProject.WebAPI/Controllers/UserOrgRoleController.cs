using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypeFormProject.Interfaces.IService;
using DTO = TypeFormProject.Models.DTOs;
namespace TypeFormProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserOrgRoleController : ControllerBase
    {
        private readonly ILogger<UserOrgRoleController> _logger;
        private readonly IUserOrgRoleService _service;
        public UserOrgRoleController(ILogger<UserOrgRoleController> logger, IUserOrgRoleService service)
        {
            _logger = logger;
            _service = service;
        }
        [HttpPost("assign")]
        public async Task<IActionResult> AssignAsync([FromBody] DTO.AssignUserOrgRole dto)
        {
            try
            {
                var result = await _service.AssignAsync(dto);
                _logger.LogInformation("Assigned role {Role} to user {UserId} in organization {OrganizationId}", dto.Role, dto.UserId, dto.OrganizationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {Role} to user {UserId} in organization {OrganizationId}", dto.Role, dto.UserId, dto.OrganizationId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while assigning the role.");
            }
        }
        [HttpGet("organization/{organizationId:int}")]
        public async Task<IActionResult> GetByOrganizationAsync([FromRoute] int organizationId)
        {
            try
            {
                var result = await _service.GetByOrganizationAsync(organizationId);
                _logger.LogInformation("Retrieved roles for organization {OrganizationId}", organizationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for organization {OrganizationId}", organizationId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the roles.");
            }
        }
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveAsync(int organizationId, string userId)
        {
            try
            {
                var result = await _service.RemoveAsync(organizationId, userId);
                _logger.LogInformation("Removed user {UserId} from organization {OrganizationId}", userId, organizationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from organization {OrganizationId}", userId, organizationId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the user from the organization.");
            }
        }
        [HttpPut("changerole")]
        public async Task<IActionResult> ChangeRoleAsync(int organizationId, string userId, int newRole)
        {
            try
            {
                var result = await _service.ChangeRoleAsync(organizationId, userId, newRole);
                _logger.LogInformation("Changed role for user {UserId} in organization {OrganizationId} to {NewRole}", userId, organizationId, newRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role for user {UserId} in organization {OrganizationId} to {NewRole}", userId, organizationId, newRole);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while changing the user's role.");
            }
        }
    }
}
