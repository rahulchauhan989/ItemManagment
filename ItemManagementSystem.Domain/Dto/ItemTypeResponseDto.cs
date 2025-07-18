namespace ItemManagementSystem.Domain.Dto;

public class ItemTypeResponseDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
