namespace ItemManagementSystem.Domain.Dto;

public class ItemModelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int ItemTypeId { get; set; }
    public int Quantity { get; set; }
}
