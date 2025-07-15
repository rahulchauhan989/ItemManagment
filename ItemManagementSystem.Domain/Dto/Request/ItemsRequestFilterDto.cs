namespace ItemManagementSystem.Domain.Dto.Request;

public class ItemsRequestFilterDto
{
    public string? RequestNumber { get; set; }
    public string? UserName { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";

    public string? SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
