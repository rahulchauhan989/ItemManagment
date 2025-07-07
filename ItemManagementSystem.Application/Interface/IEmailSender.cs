namespace ItemManagementSystem.Application.Interface;

public interface IEmailSender
{
        Task<bool> SendEmailAsync(string email, string subject, string htmlMessage);

}
