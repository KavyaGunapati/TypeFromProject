using Microsoft.Extensions.Logging;
using TypeFormProject.Interfaces.IManager;
using TypeFormProject.Interfaces.IService;
using DTO = TypeFormProject.Models.DTOs;

namespace TypeFormProject.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationManager _manager;
        private readonly ILogger<OrganizationService> _logger;

        public OrganizationService(IOrganizationManager manager, ILogger<OrganizationService> logger)
        {
            _manager = manager;
            _logger = logger;
        }
        public async Task<DTO.Result<DTO.Organization>> CreateAsync(DTO.CreateOrganization dto)
        {
            try
            {
                _logger.LogInformation("Service: Create organization request: {Name}", dto.Name);
                var res = await _manager.CreateAsync(dto);
                _logger.LogInformation("Service: Create organization result: success={Success}, msg={Message}", res.Success, res.Message);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error: Create organization {Name}", dto.Name);
                return new DTO.Result<DTO.Organization>
                {
                    Success = false,
                    Message = "Service error while creating organization."
                };
            }
        }
        public async Task<DTO.Result<DTO.Organization>> UpdateAsync(DTO.UpdateOrganization dto)
        {
            try
            {
                _logger.LogInformation("Service: Update organization request: {Id} {Name}", dto.Id, dto.Name);
                var res = await _manager.UpdateAsync(dto);
                _logger.LogInformation("Service: Update organization result: success={Success}, msg={Message}", res.Success, res.Message);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error: Update organization {Id}", dto.Id);
                return new DTO.Result<DTO.Organization>
                {
                    Success = false,
                    Message = "Service error while updating organization."
                };
            }
        }
        public async Task<DTO.Result> SoftDeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Service: Soft delete organization request: {Id}", id);
                var res = await _manager.SoftDeleteAsync(id);
                _logger.LogInformation("Service: Soft delete organization result: success={Success}, msg={Message}", res.Success, res.Message);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error: Soft delete organization {Id}", id);
                return new DTO.Result
                {
                    Success = false,
                    Message = "Service error while deleting organization."
                };
            }
        }
        public async Task<DTO.Result<DTO.Organization>> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Service: Get organization by id request: {Id}", id);
                var res = await _manager.GetByIdAsync(id);
                _logger.LogInformation("Service: Get organization by id result: success={Success}, msg={Message}", res.Success, res.Message);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error: Get organization by id {Id}", id);
                return new DTO.Result<DTO.Organization>
                {
                    Success = false,
                    Message = "Service error while retrieving organization."
                };
            }
        }
        public async Task<DTO.Result<List<DTO.Organization>>> GetAllAsync(bool includeDeleted = false)
        {
            try
            {
                _logger.LogInformation("Service: Get all organizations request includeDeleted={Include}", includeDeleted);
                var res = await _manager.GetAllAsync(includeDeleted);
                _logger.LogInformation("Service: Get all organizations result: success={Success}, count={Count}",
                    res.Success, res.Data?.Count ?? 0);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error: List organizations.");
                return new DTO.Result<List<DTO.Organization>>
                {
                    Success = false,
                    Message = "Service error while listing organizations."
                };
            }
        }
    }
}
