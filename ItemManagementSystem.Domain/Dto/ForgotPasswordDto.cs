using System.ComponentModel.DataAnnotations;

namespace ItemManagementSystem.Domain.Dto;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
} 
