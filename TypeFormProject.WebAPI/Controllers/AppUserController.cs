using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypeFormProject.Interfaces.IService;
using TypeFormProject.Models.DTOs;

namespace TypeFormProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
            private readonly IAppUserService _authService;
            private readonly ILogger<AppUserController> _logger;

            public AppUserController(IAppUserService authService, ILogger<AppUserController> logger)
            {
                _authService = authService;
                _logger = logger;
            }

            [HttpPost("refresh")]
            public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest req)
            {
            try
            {
                if (req is null || string.IsNullOrWhiteSpace(req.RefreshToken))
                    return BadRequest(new { errors = "Refresh token is required." });

                var result = await _authService.RefreshAsync(req.RefreshToken);

                if (!result.Success)
                {
                    _logger.LogWarning("Refresh endpoint failed: {Message}", result.Message);
                    return Unauthorized(new { errors = result.Message });
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] Register register)
        {
            try
            {
                var result = await _authService.SignUpAsync(register);
                if (!result.Success)
                {
                    _logger.LogWarning("SignUp failed: {Message}", result.Message);
                    return BadRequest(new { errors = result.Message });
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            try
            {
                var result = await _authService.LoginAsync(login);
                if (!result.Success)
                {
                    _logger.LogWarning("Login failed: {Message}", result.Message);
                    return Unauthorized(new { errors = result.Message });
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest req)
        {
            try
            {
                var result = await _authService.LogoutAsync(req.RefreshToken);
                if (!result.Success)
                {
                    _logger.LogWarning("Logout failed: {Message}", result.Message);
                    return BadRequest(new { errors = result.Message });
                }
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

    }
}


