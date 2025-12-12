using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity=TypeFormProject.DataAccess.Entities;
using TypeFormProject.DataAccess.Enums;
using TypeFormProject.Interfaces.IManager;
using TypeFormProject.Interfaces.IRepository;
using DTO=TypeFormProject.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace TypeFormProject.Services
{
    public class UserOrgRoleManager : IUserOrgRoleManager
    {
        private readonly IGenericRepository<Entity.UserOrgRole> _uorRepo;
        private readonly IGenericRepository<Entity.Organization> _orgRepo;
        private readonly UserManager<Entity.AppUser> _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserOrgRoleManager> _logger;

        public UserOrgRoleManager(
            IGenericRepository<Entity.UserOrgRole> uorRepo,
            IGenericRepository<Entity.Organization> orgRepo,
            UserManager<Entity.AppUser> userRepo,
            IMapper mapper,
            ILogger<UserOrgRoleManager> logger)
        {
            _uorRepo = uorRepo;
            _orgRepo = orgRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DTO.Result<DTO.UserOrgRole>> AssignAsync(DTO.AssignUserOrgRole dto)
        {
            try
            {
                var org = await _orgRepo.GetByIdAsync(dto.OrganizationId);
                if (org == null || org.IsDeleted)
                {
                    return new DTO.Result<DTO.UserOrgRole>
                    {
                        Success = false,
                        Message = "Organization not found or deleted."
                    };
                }

                var user = await _userRepo.FindByIdAsync(dto.UserId);
                if (user == null || !user.IsActive)
                {
                    return new DTO.Result<DTO.UserOrgRole>
                    {
                        Success = false,
                        Message = "User not found or inactive."
                    };
                }

                bool exists = await _uorRepo.Query()
                    .AnyAsync(x => x.OrganizationId == dto.OrganizationId && x.UserId == dto.UserId);
                if (exists)
                {
                    _logger.LogWarning("Attempt to reassign user {UserId} to org {OrgId}", dto.UserId, dto.OrganizationId);
                    return new DTO.Result<DTO.UserOrgRole>
                    {
                        Success = false,
                        Message = "User already assigned to this organization."
                    };
                }

                if (!Enum.IsDefined(typeof(OrgRole), dto.Role))
                {
                    _logger.LogWarning("Attempt to assign invalid role {Role} to user {UserId} in org {OrgId}", dto.Role, dto.UserId, dto.OrganizationId);
                    return new DTO.Result<DTO.UserOrgRole>
                    {
                        Success = false,
                        Message = "Invalid role."
                    };
                }

                _logger.LogInformation("Attempting to assign role {Role} to user {UserId} in org {OrgId}", dto.Role, dto.UserId, dto.OrganizationId);
                var entity = new Entity.UserOrgRole
                {
                    OrganizationId = dto.OrganizationId,
                    UserId = dto.UserId,
                    Role = (OrgRole)dto.Role
                };

                await _uorRepo.AddAsync(entity);
                await _uorRepo.SaveChangesAsync();
                _logger.LogInformation("Assigned role {Role} to user {UserId} in org {OrgId}", entity.Role, dto.UserId, dto.OrganizationId);
                return new DTO.Result<DTO.UserOrgRole>
                {
                    Success = true,
                    Data = _mapper.Map<DTO.UserOrgRole>(entity),
                    Message = "Role assigned."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user {UserId} in org {OrgId}", dto.UserId, dto.OrganizationId);
                return new DTO.Result<DTO.UserOrgRole>
                {
                    Success = false,
                    Message = "An unexpected error occurred while assigning user role."
                };
            }
        }

        public async Task<DTO.Result<List<DTO.UserOrgRole>>> GetByOrganizationAsync(int organizationId)
        {
            try
            {
                var roles = await _uorRepo.FindAsync(x => x.OrganizationId == organizationId);

                _logger.LogInformation("Roles listed for org {OrgId}, count={Count}", organizationId, roles.Count);
                return new DTO.Result<List<DTO.UserOrgRole>>
                {
                    Success = true,
                    Data = _mapper.Map<List<DTO.UserOrgRole>>(roles),
                    Message = "Roles retrieved."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing roles for org {OrgId}", organizationId);
                return new DTO.Result<List<DTO.UserOrgRole>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while listing roles."
                };
            }
        }

        public async Task<DTO.Result> RemoveAsync(int organizationId, string userId)
        {

            try
            {
                var entity = await _uorRepo.Query()
                    .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId);

                if (entity == null)
                {
                    _logger.LogWarning("Attempt to remove non-existent assignment of user {UserId} from org {OrgId}", userId, organizationId);
                    return new DTO.Result
                    {
                        Success = false,
                        Message = "Assignment not found."
                    };
                }

                await _uorRepo.DeleteAsync(entity);
                await _uorRepo.SaveChangesAsync();
                _logger.LogInformation("Removed user {UserId} from org {OrgId}", userId, organizationId);
                return new DTO.Result
                {
                    Success = true,
                    Message = "User removed from organization."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from org {OrgId}", userId, organizationId);
                return new DTO.Result
                {
                    Success = false,
                    Message = "An unexpected error occurred while removing user from organization."
                };
            }
        }

        public async Task<DTO.Result<DTO.UserOrgRole>> ChangeRoleAsync(int organizationId, string userId, int newRole)
        {
            try
            {
                var entity = await _uorRepo.Query()
                    .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId);

                if (entity == null)
                {
                    _logger.LogWarning("Attempt to change role for non-existent assignment of user {UserId} in org {OrgId}", userId, organizationId);
                    return new DTO.Result<DTO.UserOrgRole>
                    {
                        Success = false,
                        Message = "Assignment not found."
                    };
                }

                if (!Enum.IsDefined(typeof(OrgRole), newRole))
                {
                    _logger.LogWarning("Attempt to change to invalid role {Role} for user {UserId} in org {OrgId}", newRole, userId, organizationId);
                    return new DTO.Result<DTO.UserOrgRole>
                    {
                        Success = false,
                        Message = "Invalid role."
                    };
                }

                entity.Role = (OrgRole)newRole;
                await _uorRepo.UpdateAsync(entity);
                await _uorRepo.SaveChangesAsync();
                _logger.LogInformation("Changed role for user {UserId} in org {OrgId} to {Role}", userId, organizationId, entity.Role);
                return new DTO.Result<DTO.UserOrgRole>
                {
                    Success = true,
                    Data = _mapper.Map<DTO.UserOrgRole>(entity),
                    Message = "User role changed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing role for user {UserId} in org {OrgId}", userId, organizationId);
                return new DTO.Result<DTO.UserOrgRole>
                {
                    Success = false,
                    Message = "An unexpected error occurred while changing role."
                };
            }
        }
    }
}
