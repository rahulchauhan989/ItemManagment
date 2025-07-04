namespace ItemManagementSystem.Domain.Dto;

public class ResetPasswordDto
{
    public string? Token { get; set; }
    public string? NewPassword { get; set; }
}
