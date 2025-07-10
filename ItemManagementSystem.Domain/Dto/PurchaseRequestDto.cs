namespace ItemManagementSystem.Domain.Dto
{
    public class PurchaseRequestDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? InvoiceNumber { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PurchaseRequestItemDto> Items { get; set; } = null!;
    }
}

public class PurchaseRequestItemDto
{
    public int ItemModelId { get; set; }
    public string? Name { get; set; }
    public string? ItemType { get; set; }
    public int Quantity { get; set; }
    public int ItemTypeId { get; set; }
}