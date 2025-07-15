namespace ItemManagementSystem.Domain.Dto.Request;

public class ItemRequestFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}