using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TypeFormProject.Interfaces.IManager;
using TypeFormProject.Interfaces.IRepository;

using DTO=TypeFormProject.Models.DTOs;
using Entity=TypeFormProject.DataAccess.Entities;

namespace TypeFormProject.Managers
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly IGenericRepository<Entity.Organization> _orgRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<OrganizationManager> _logger;

        public OrganizationManager(
            IGenericRepository<Entity.Organization> orgRepo,
            IMapper mapper,
            ILogger<OrganizationManager> logger)
        {
            _orgRepo = orgRepo;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<DTO.Result<DTO.Organization>> CreateAsync(DTO.CreateOrganization dto)
        {
            try
            {
                bool exists = await _orgRepo.Query()
                    .AnyAsync(o => o.Name == dto.Name && !o.IsDeleted);

                if (exists)
                {
                    _logger.LogWarning("Organization already exist: {Name}",  dto.Name);
                    return new DTO.Result<DTO.Organization> { Message = $"Organization '{dto.Name}' already exists." , Success = false };
                }

                var entity = _mapper.Map<Entity.Organization>(dto);
                await _orgRepo.AddAsync(entity);
                await _orgRepo.SaveChangesAsync();
              var data = _mapper.Map<DTO.Organization>(entity);
                _logger.LogInformation("Organization created: {OrgId} {Name}", entity.Id, entity.Name);
                return new DTO.Result<DTO.Organization> { Success = true, Message = "Organization created." ,Data=data};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating organization: {Name}", dto.Name);
                return new DTO.Result<DTO.Organization> { Success = false, Message = "An unexpected error occurred while creating the organization." };
            }
           
        }

        public async Task<DTO.Result<DTO.Organization>> UpdateAsync(DTO.UpdateOrganization dto)
        {
            try
            {
                var entity = await _orgRepo.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Organization not found: {Name}", dto.Name);
                    return new DTO.Result<DTO.Organization> { Success = false, Message = "Organization not found." };
                }

                if (!string.IsNullOrWhiteSpace(dto.Name) && !string.Equals(entity.Name, dto.Name, StringComparison.Ordinal))
                {
                    bool taken = await _orgRepo.Query()
                        .AnyAsync(o => o.Name == dto.Name && o.Id != dto.Id && !o.IsDeleted);
                    if (taken)
                    {
                        _logger.LogWarning("Another Organization already used this name: {Name}", dto.Name);
                        return new DTO.Result<DTO.Organization> { Success = false , Message = $"Another organization already uses '{dto.Name}'." };
                    }
                }
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    entity.Name = dto.Name;

                if (dto.IsDeleted.HasValue)
                    entity.IsDeleted = dto.IsDeleted.Value;

                await _orgRepo.UpdateAsync(entity);
                await _orgRepo.SaveChangesAsync();
                var data = _mapper.Map<DTO.Organization>(entity);
                _logger.LogInformation("Organization updated: {OrgId}", entity.Id);
                return new DTO.Result<DTO.Organization> { Success = true, Message = "Organization updated.", Data = data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization: {Id}", dto.Id);
                return new DTO.Result<DTO.Organization> { Success = false, Message = "An unexpected error occurred while updating the organization." };
            }
        }
        public async Task<DTO.Result> SoftDeleteAsync(int id)
        {
            try
            {
                var entity = await _orgRepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new DTO.Result { Success = false, Message = "Organization not found." };
                }
                if (entity.IsDeleted)
                {
                    return new DTO.Result { Success = true, Message = "Organization already deleted." };
                }
                entity.IsDeleted = true;
                await _orgRepo.UpdateAsync(entity);
                await _orgRepo.SaveChangesAsync();

                _logger.LogInformation("Organization soft deleted: {Id}", id);
                return new DTO.Result
                {
                    Success = true,
                    Message = "Organization soft deleted."
                };

                }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting organization: {Id}", id);
                return new DTO.Result { Success = false, Message = "An unexpected error occurred while deleting the organization." };
            }
        }
        public async Task<DTO.Result<DTO.Organization>> GetByIdAsync(int id)
        {

            try
            {
                var entity = await _orgRepo.GetByIdAsync(id);
                if (entity == null)
                {
                    return new DTO.Result<DTO.Organization> { Success = false, Message = "Organization not found." };
                }
                var data = _mapper.Map<DTO.Organization>(entity);
                _logger.LogInformation("Organization fetched: {Id}", id);
                return new DTO.Result<DTO.Organization> { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching organization: {Id}", id);


                return new DTO.Result<DTO.Organization>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the organization."
                };
                }
        }
        public async Task<DTO.Result<List<DTO.Organization>>> GetAllAsync(bool includeDeleted = false)
        {
            
            try
            {
                var list = await _orgRepo.FindAsync(o => includeDeleted || !o.IsDeleted);
               
                var data = _mapper.Map<List<DTO.Organization>>(list);
                _logger.LogInformation("Organizations listed. includeDeleted={Include}", includeDeleted);
                return new DTO.Result<List<DTO.Organization>> { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing organizations.");

                return new DTO.Result<List<DTO.Organization>> { Success = false, Message = "An unexpected error occurred while listing organizations." };
            }
            
        }
    }
}
