using ItemManagementSystem.Domain.Dto;

namespace ItemManagementSystem.Application.Interface;

public interface IItemRequestService
{
    Task<IEnumerable<ItemRequestDto>> GetPendingRequestsAsync(ItemRequestFilterDto filter);
    Task ApproveRequestAsync(int id, string comment);
    Task RejectRequestAsync(int id, string comment);
}
