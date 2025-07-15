namespace ItemManagementSystem.Domain.Dto
{
    public class PurchaseRequestFilterDto
    {
        // public string? InvoiceNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? Date { get; set; } = null;
        public string? SortBy { get; set; } = "Date";
        public string? SortDirection { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}