using TypeFormProject.Models.DTOs;

namespace TypeFormProject.Interfaces.IManager
{
    public interface IAppUserManager
    {
        Task<Result<AuthResponse>> SignUp(Register register);
        Task<Result<AuthResponse>> Login(Login login);
        Task<Result<bool>> Logout();
        Task<Result<AuthResponse>> RefreshAsync(string refreshToken);
    }
}
