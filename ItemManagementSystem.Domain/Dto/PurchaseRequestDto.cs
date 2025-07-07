namespace ItemManagementSystem.Domain.Dto;

public class PurchaseRequestDto
{
    public List<PurchaseRequestItemDto> Items { get; set; } = null!;
}
public class PurchaseRequestItemDto
{
    public int ItemModelId { get; set; }
    public int Quantity { get; set; }
}