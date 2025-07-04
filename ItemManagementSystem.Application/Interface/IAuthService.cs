namespace ItemManagementSystem.Application.Interface;

public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task ForgotPasswordAsync(string email); // Send reset link or token
        Task ResetPasswordAsync(string token, string newPassword);
    }
