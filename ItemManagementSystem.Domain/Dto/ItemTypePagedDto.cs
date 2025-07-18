namespace ItemManagementSystem.Domain.Dto;

public class ItemTypePagedDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
