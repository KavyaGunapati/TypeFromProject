
using AutoMapper;
using EcommerceWebAPI.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TypeFormProject.DataAccess.Entities;
using TypeFormProject.Interfaces.IManager;
using TypeFormProject.Interfaces.IRepository;
using TypeFormProject.Models.DTOs;
using TypeFormProject.Models.JwtTokenService;

namespace TypeFormProject.Managers
{
    public class AppUserManager : IAppUserManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppUserManager> _logger;
        private const string DefaultRole = "User";
        private readonly IConfiguration _config;

        private readonly IGenericRepository<RefreshToken> _repository;
        public AppUserManager(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService,
            IMapper mapper,
            ILogger<AppUserManager> logger,IConfiguration config,IGenericRepository<RefreshToken> repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _repository = repository;
        }

        public async Task<Result<AuthResponse>> SignUp(Register register)
        {
            try
            {
                _logger.LogInformation("SignUp started for email: {Email}", register.Email);

                var existing = await _userManager.FindByEmailAsync(register.Email);
                if (existing is not null)
                {
                    _logger.LogWarning("SignUp failed: user with email {Email} already exists.", register.Email);
                    return new Result<AuthResponse> {Success=false, Message= "Email already registered." };
                }

                var user = _mapper.Map<AppUser>(register);
                var create = await _userManager.CreateAsync(user, register.Password);
                if (!create.Succeeded)
                {
                    var errors = string.Join(" ", create.Errors.Select(e => e.Description));
                    _logger.LogWarning("SignUp failed for {Email}: {Errors}", register.Email, string.Join("; ", errors));
                    return new Result<AuthResponse> {Success=false,Message=errors};
                }

                if (!await _roleManager.RoleExistsAsync(DefaultRole))
                {
                    var roleCreate = await _roleManager.CreateAsync(new IdentityRole(DefaultRole));
                    if (!roleCreate.Succeeded)
                    {
                        var errors = string.Join(" ", roleCreate.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create default role {Role}.", DefaultRole);
                        return new Result<AuthResponse> { Message = $"Failed to create default role. Check errors={errors}" ,Success=false};
                    }
                }

                await _userManager.AddToRoleAsync(user, DefaultRole);

                // Build roles and generate tokens
                var roles = await _userManager.GetRolesAsync(user);
                var tokenData = new UserTokenData
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles,
                    UserName=user.Email,

                    
                };
                var accessToken = _tokenService.GenerateToken(
                   tokenData
                );
                var (refreshToken, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

                var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"]!);
                var auth = new AuthResponse
                {
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessMinutes),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiresAt = refreshExpiresAt,
                   UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!
                };
                await SaveRefreshTokenAsync(auth.UserId!, auth.RefreshToken, auth.RefreshTokenExpiresAt);
                _logger.LogInformation("SignUp successful for {Email}", register.Email);
                return new Result<AuthResponse> { Success=true, Message="Registered Successfully",Data=auth};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignUp exception for email: {Email}", register.Email);
                return new Result<AuthResponse> {Success=false, Message = "Unexpected error during sign up." };
            }
        }

        public async Task<Result<AuthResponse>> Login(Login login)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", login.Email);

                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user is null)
                {
                    _logger.LogWarning("Login failed: user {Email} not found.", login.Email);
                    return new Result<AuthResponse> { Success = false, Message = "Invalid credentials." };
                }

                var pwd = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: false);
                if (!pwd.Succeeded)
                {
                    _logger.LogWarning("Login failed: wrong password for {Email}.", login.Email);
                    return new Result<AuthResponse> { Success = false, Message = ErrorConstants.InvalidCredentials };
                }
                var roles = await _userManager.GetRolesAsync(user);
                var tokenData = new UserTokenData
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles,
                    UserName = user.Email

                };
                var accessToken = _tokenService.GenerateToken(
                 tokenData
              );
                var (refreshToken, refreshExpiresAt) = _tokenService.GenerateRefreshToken();

                var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"]!);
                var auth = new AuthResponse
                {
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessMinutes),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiresAt = refreshExpiresAt,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!
                };
               
                await SaveRefreshTokenAsync(auth.UserId!, auth.RefreshToken, auth.RefreshTokenExpiresAt);
                _logger.LogInformation("Login succeeded for {Email}", login.Email);
                return new Result<AuthResponse> { Success = true, Message = "Logged in Successfully", Data =auth };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login exception for email: {Email}", login.Email);
                return new Result<AuthResponse> { Message = "Unexpected error during login.", Success = false };
            }
        }

        public async Task<Result<bool>> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync(); 
                _logger.LogInformation("Logout called and signout executed.");
                return new Result<bool> { Success = true , Message="Logout successfull"};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout exception.");
                return new Result<bool> { Success = false, Message = "Unexpected error during logout." };
            }
        }
        public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = (await _repository.GetAllAsync())
                    .FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);

                if (tokenEntity is null || tokenEntity.ExpiresAt <= DateTime.UtcNow)
                {
                    _logger.LogWarning("RefreshAsync: invalid or expired refresh token.");
                    return new Result<AuthResponse> { Success = false, Message = "Invalid or expired refresh token." };
                }


                tokenEntity.IsRevoked = true;

                var user = tokenEntity.User!;
                var roles = await _userManager.GetRolesAsync(user);

                var tokenData = new UserTokenData
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.Email ?? user.UserName ?? user.Id,
                    Roles = roles
                };

                var accessToken = _tokenService.GenerateToken(tokenData);
                var (newRefresh, newRefreshExp) = _tokenService.GenerateRefreshToken();
                var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"]!);
                await SaveRefreshTokenAsync(user.Id, newRefresh, newRefreshExp);

                var auth = new AuthResponse
                {
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessMinutes),
                    RefreshToken = newRefresh,
                    RefreshTokenExpiresAt = newRefreshExp,

                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!
                };

                _logger.LogInformation("RefreshAsync: new tokens issued.");
                return new Result<AuthResponse> { Success = true, Message = "Token refreshed.", Data = auth };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshAsync exception.");
                return new Result<AuthResponse> { Success = false, Message = "Unexpected error during token refresh." };
            }
        }
        private async Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTime expiresAt)
        {
            var entity = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

        }
    }
}
