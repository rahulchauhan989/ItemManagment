namespace ItemManagementSystem.Domain.Dto.Request;

public class ItemRequestWithIdsResponseDto
{
    public int Id { get; set; }
    public string? RequestNumber { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<RequestItemWithIdsResponseDto> Items { get; set; } = new List<RequestItemWithIdsResponseDto>();
}

public class RequestItemWithIdsResponseDto
{
    public int ItemModelId { get; set; }
    public int ItemTypeId { get; set; }
    public string? ItemModelName { get; set; }
    public string? ItemTypeName { get; set; }
    public int Quantity { get; set; }
}
