namespace ItemManagementSystem.Domain.Dto;

public class PurchaseRequestFilterDto
{
    public string? RequestNumber { get; set; }
    public string? UserName { get; set; }
    public string? Status { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public bool? Ascending { get; set; }
}