using System.ComponentModel.DataAnnotations;

namespace ItemManagementSystem.Domain.Dto;

public class ItemModelResponseDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Name can only contain letters, numbers, and spaces.")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int ItemTypeId { get; set; }
    public int Quantity { get; set; }
    public string? ItemTypeName { get; set; }
}
