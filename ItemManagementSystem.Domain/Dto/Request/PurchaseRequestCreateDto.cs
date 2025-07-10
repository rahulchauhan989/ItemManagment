namespace ItemManagementSystem.Domain.Dto.Request;

public class PurchaseRequestCreateDto
{
    // public DateTime Date { get; set; }
    public List<PurchaseRequestItemCreateDto> Items { get; set; } = null!;
}

public class PurchaseRequestItemCreateDto
{
    public int ItemModelId { get; set; }
    public int Quantity { get; set; }
}