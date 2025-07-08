namespace ItemManagementSystem.Domain.Dto;

public class ItemTypeDto
{
    public int? Id { get; set; } 
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? createdBy { get; set; }
    public int? modifiedBy { get; set; }
}
