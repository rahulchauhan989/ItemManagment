using ItemManagementSystem.Domain.Dto;

namespace ItemManagementSystem.Application.Interface;

public interface IUserItemRequestService
{
    Task<ItemRequestResponseDto> CreateRequestAsync(int userId, CreateItemRequestDto dto);
    Task<List<ItemRequestResponseDto>> GetRequestsByUserAsync(int userId);
    Task<bool> ChangeStatusAsync(int requestId, string newStatus, int userId);
}
