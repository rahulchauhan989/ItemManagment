namespace ItemManagementSystem.Domain.Dto;

public class RequestItemsDto
{
    public int ItemModelId { get; set; }
    public int Quantity { get; set; }
    public string? ItemModelName { get; set; }
    public string? ItemModelDescription { get; set; }
    public int ItemTypeId { get; set; }
    public string? ItemTypeName { get; set; }
}
