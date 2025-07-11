using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Application.Interface;

public interface IItemRequestService
{
    Task<PagedResultDto<ItemRequestDto>> GetRequestsAsync(ItemsRequestFilterDto filter);
    Task ChangeRequestStatusAsync(int id, string status, string? comment, int userId);
}
