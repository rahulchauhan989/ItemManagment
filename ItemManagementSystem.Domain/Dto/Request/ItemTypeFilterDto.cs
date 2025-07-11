namespace ItemManagementSystem.Domain.Dto.Request;

 public class ItemTypeFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; } = "Name";
        public bool SortDesc { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }