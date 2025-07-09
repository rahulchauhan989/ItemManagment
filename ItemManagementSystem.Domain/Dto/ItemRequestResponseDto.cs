namespace ItemManagementSystem.Domain.Dto;

public class ItemRequestResponseDto
{
    public int Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<RequestItemDto> Items { get; set; } = new();
}
