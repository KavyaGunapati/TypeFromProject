using TypeFormProject.Models.DTOs;

namespace TypeFormProject.Interfaces.IService
{

    public interface IAppUserService
    {
        Task<Result<AuthResponse>> SignUpAsync(Register register);
        Task<Result<AuthResponse>> LoginAsync(Login login);
        Task<Result<bool>> LogoutAsync(string refreshToken);
        Task<Result<AuthResponse>> RefreshAsync(string refreshToken);
    }
}


