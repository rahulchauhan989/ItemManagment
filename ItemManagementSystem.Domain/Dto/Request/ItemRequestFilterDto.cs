namespace ItemManagementSystem.Domain.Dto.Request;

public class ItemRequestFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? OrderBy { get; set; } = "CreatedAt";
    public bool SortDesc { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}