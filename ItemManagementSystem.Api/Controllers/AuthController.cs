using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;

        public AuthController(IAuthService authService, IEmailSender emailSender)
        {
            _authService = authService;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            return new ApiResponse(true, 200, token, AppMessages.LoginSuccess);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var token = _authService.GenerateJwtToken(dto.Email);

            string resetLink = Url.Action("ResetPassword", "Auth", new { token = Uri.EscapeDataString(token) }, Request.Scheme)!;

            string emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            await _emailSender.SendEmailAsync(dto.Email, "Reset Password", emailBody);

            return new ApiResponse(true, 200, null, AppMessages.ResetLinkSent);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
             await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);

            return new ApiResponse(true, 200, null, AppMessages.PasswordResetSuccess);
        }
    }
}