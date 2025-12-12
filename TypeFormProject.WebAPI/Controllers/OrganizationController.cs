using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypeFormProject.Interfaces.IService;
using DTO = TypeFormProject.Models.DTOs;

namespace TypeFormProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _service;
        private readonly ILogger<OrganizationController> _logger;

        public OrganizationController(IOrganizationService service, ILogger<OrganizationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get_all_organizations")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            try
            {
                var res = await _service.GetAllAsync(includeDeleted);
                if (res == null)
                {
                    _logger.LogWarning("No organizations found when fetching all organizations");
                    return NotFound(new DTO.Result { Success = false, Message = "No organizations found." });
                }
               _logger.LogInformation("All organizations retrieved, count: {Count}", res.Data!.Count);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all organizations");
                return StatusCode(500, new DTO.Result { Success = false, Message = "An unexpected error occurred while retrieving organizations." });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var res = await _service.GetByIdAsync(id);
                if (res == null)
                {
                    _logger.LogWarning("Organization with ID {Id} not found", id);
                    return NotFound(new DTO.Result { Success = false, Message = "Organization not found." });
                }
               _logger.LogInformation("Organization with ID {Id} retrieved", id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching organization by ID: {Id}", id);
                return StatusCode(500, new DTO.Result { Success = false, Message = "An unexpected error occurred while retrieving the organization." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.CreateOrganization dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new DTO.Result { Success = false, Message = $"{ModelState}" });
                }

                var res = await _service.CreateAsync(dto);
                if (res.Success)
                {
                    _logger.LogInformation("Organization created with ID {Id}", res.Data!.Id);
                    return CreatedAtAction(nameof(GetById), new { id = res.Data!.Id }, res);
                }
                _logger.LogWarning("Failed to create organization: {Message}", res.Message);
                return BadRequest(res);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating organization");
                return StatusCode(500, new DTO.Result { Success = false, Message = "An unexpected error occurred while creating the organization." });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DTO.UpdateOrganization dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new DTO.Result { Success = false, Message = $"{ModelState}" });

            try
            {
                dto.Id = id;
                var res = await _service.UpdateAsync(dto);
                if (!res.Success)
                {
                    _logger.LogWarning("Failed to update organization: {Message}", res.Message);
                    return BadRequest(res);
                }
                    _logger.LogInformation("Organization updated with ID {Id}", id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization with ID: {Id}", id);
                return StatusCode(500, new DTO.Result { Success = false, Message = "An unexpected error occurred while updating the organization." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete([FromRoute] int id)
        {
            try
            {
                var res = await _service.SoftDeleteAsync(id);
                if (!res.Success)
                {
                    _logger.LogWarning("Failed to soft delete organization: {Message}", res.Message);
                    return BadRequest(res);
                }
                _logger.LogInformation("Organization soft deleted with ID {Id}", id);
                return NoContent();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting organization with ID: {Id}", id);
                return StatusCode(500, new DTO.Result { Success = false, Message = "An unexpected error occurred while deleting the organization." });
            }
        }

       
    }
}

