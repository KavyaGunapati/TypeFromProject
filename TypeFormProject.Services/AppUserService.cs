
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TypeFormProject.DataAccess.Context;
using TypeFormProject.DataAccess.Entities;
using TypeFormProject.Interfaces.IManager;
using TypeFormProject.Interfaces.IRepository;
using TypeFormProject.Interfaces.IService;
using TypeFormProject.Models.DTOs;
using TypeFormProject.Models.JwtTokenService;

namespace TypeFormProject.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserManager _manager;
        private readonly IGenericRepository<RefreshToken> _repository;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<AppUserService> _logger;

        public AppUserService(
            IAppUserManager manager,
            AppDbContext context,
            UserManager<AppUser> userManager,
            TokenService tokenService,
            ILogger<AppUserService> logger, IGenericRepository<RefreshToken> repository)
        {
            _manager = manager;
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _repository = repository;
        }

        public async Task<Result<AuthResponse>> SignUpAsync(Register register)
        {
            try
            {
                var result = await _manager.SignUp(register);
                if (!result.Success)
                    return new Result<AuthResponse> { Success = false, Message = string.Join("; ", result.Message) };
                return new Result<AuthResponse> { Success = true, Message = "Registered successfully.", Data = result.Data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignUpAsync exception.");
                return new Result<AuthResponse> { Success = false, Message = "Unexpected error during sign up." };
            }
        }

        public async Task<Result<AuthResponse>> LoginAsync(Login login)
        {
            try
            {
                var result = await _manager.Login(login);
                if (!result.Success)
                    return new Result<AuthResponse> { Success = false, Message = result.Message };
                return new Result<AuthResponse> { Success = true, Message = "Logged in successfully.", Data = result.Data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginAsync exception.");
                return new Result<AuthResponse> { Success = false, Message = "Unexpected error during login." };
            }
        }

        public async Task<Result<bool>> LogoutAsync(string refreshToken)
        {
            try
            {

                var token = (await _repository.GetAllAsync()).FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);
                if (token is null)
                {
                    _logger.LogWarning("LogoutAsync: refresh token not found or already revoked.");
                    return new Result<bool> { Success = true, Message = "Already logged out." };
                }

                token.IsRevoked = true;
                await _repository.SaveChangesAsync();

                _logger.LogInformation("LogoutAsync: refresh token revoked.");
                return new Result<bool> { Success = true, Message = "Logout successful.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogoutAsync exception.");
                return new Result<bool> { Success = false, Message = "Unexpected error during logout." };
            }
        }

        public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken)
        {
            try
            {
                var auth = await _manager.RefreshAsync(refreshToken);


                _logger.LogInformation("RefreshAsync: new tokens issued.");
                return new Result<AuthResponse> { Success = true, Message = "Token refreshed.", Data = auth.Data };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshAsync exception.");
                return new Result<AuthResponse> { Success = false, Message = "Unexpected error during token refresh." };
            }
        }


    }
}
