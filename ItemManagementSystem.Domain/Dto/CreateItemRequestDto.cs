
namespace ItemManagementSystem.Domain.Dto;

using ItemManagementSystem.Domain.Dto.Request;

public class CreateItemRequestDto
{
    public List<CreateRequestItemDto> Items { get; set; } = new List<CreateRequestItemDto>();
}
