
namespace ItemManagementSystem.Domain.Dto;

public class CreateItemRequestDto
{
    public List<RequestItemDto> Items { get; set; } = new List<RequestItemDto>();
}