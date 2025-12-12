
using DTO = TypeFormProject.Models.DTOs;

namespace TypeFormProject.Interfaces.IService
{
    public interface IOrganizationService
    {
        Task<DTO.Result<DTO.Organization>> CreateAsync(DTO.CreateOrganization dto);
        Task<DTO.Result<DTO.Organization>> UpdateAsync(DTO.UpdateOrganization dto);
        Task<DTO.Result> SoftDeleteAsync(int id);
        Task<DTO.Result<DTO.Organization>> GetByIdAsync(int id);
        Task<DTO.Result<List<DTO.Organization>>> GetAllAsync(bool includeDeleted = false);
    }
}
