namespace ItemManagementSystem.Application.Interface;

public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        string GenerateJwtToken(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> isEmailExist(string email);
    }
