namespace ItemManagementSystem.Domain.Dto.Request;

public class ReturnRequestCreateDto
{
    public List<ReturnRequestItemDto> Items { get; set; } = new();
}

public class ReturnRequestItemDto
{
    public int ItemModelId { get; set; }
    public int Quantity { get; set; }
}

public class ReturnRequestFilterDto
{
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ReturnRequestDto
{
    public int Id { get; set; }
    public string ReturnRequestNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public List<ReturnRequestItemDto> Items { get; set; } = new();
}