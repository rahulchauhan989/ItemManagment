namespace ItemManagementSystem.Domain.Dto.Request;

public class ItemModelFilterDto
{
    public string? SearchTerm { get; set; }
    public int? ItemTypeId { get; set; }  
    public string? OrderBy { get; set; } = "Name";
    public bool SortDesc { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}