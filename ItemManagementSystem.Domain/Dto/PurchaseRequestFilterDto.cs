namespace ItemManagementSystem.Domain.Dto
{
    public class PurchaseRequestFilterDto
    {
        public string? InvoiceNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? Date { get; set; }
        public string? SortBy { get; set; } = "Date";
        public bool SortDesc { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}