using System.ComponentModel.DataAnnotations;

namespace ItemManagementSystem.Domain.Dto;

public class ResetPasswordDto
{
    
    public string Token { get; set; }  = null!;

    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be like eg.Abc@123 and length should be 8 to 15")]
    public string NewPassword { get; set; } = null!;

    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be like eg.Abc@123 and length should be 8 to 15")]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
