namespace ItemManagementSystem.Domain.Dto;

public class ItemTypeDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? createdBy { get; set; }
}
