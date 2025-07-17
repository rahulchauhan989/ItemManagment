using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Application.Interface;

public interface IUserItemRequestService
{
    Task<ItemRequestResponseDto> CreateRequestAsync(int userId, CreateItemRequestDto dto);
    Task<bool> ChangeStatusAsync(int requestId, int userId);
    Task<PagedResultDto<ItemRequestResponseDto>> GetRequestsByUserPagedAsync(int userId, Domain.Dto.Request.ItemRequestFilterDto filter);
    Task SaveDraftAsync(int userId, CreateItemRequestDto dto);
    Task ChangeDraftToPendingAsync(int requestId, int userId);
}
