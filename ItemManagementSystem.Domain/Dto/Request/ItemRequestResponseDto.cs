namespace ItemManagementSystem.Domain.Dto.Request;

public class ItemRequestResponseDto
{
    public int Id { get; set; }
    public string? RequestNumber { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<RequestItemResponseDto> Items { get; set; } = new List<RequestItemResponseDto>();
}
