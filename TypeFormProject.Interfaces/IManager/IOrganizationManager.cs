using TypeFormProject.Models.DTOs;

namespace TypeFormProject.Interfaces.IManager
{

    public interface IOrganizationManager
    {
        Task<Result<Organization>> CreateAsync(CreateOrganization dto);
        Task<Result<Organization>> UpdateAsync(UpdateOrganization dto);
        Task<Result> SoftDeleteAsync(int id);
        Task<Result<Organization>> GetByIdAsync(int id);
        Task<Result<List<Organization>>> GetAllAsync(bool includeDeleted = false);
    }
}


