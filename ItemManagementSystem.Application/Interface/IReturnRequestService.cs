using ItemManagementSystem.Domain.Dto.Request;
using System.Threading.Tasks;

namespace ItemManagementSystem.Application.Interface
{
    public interface IReturnRequestService
    {
        Task<ReturnRequestDto> CreateReturnRequestAsync(int userId, ReturnRequestCreateDto dto);
        Task<PagedResultDto<ReturnRequestDto>> GetUserReturnRequestsAsync(int userId, ReturnRequestFilterDto filter);
        Task UpdateReturnRequestStatusAsync(int id, string status, string? comment, int userId);
        Task<PagedResultDto<ReturnRequestDto>> GetAllReturnRequestsAsync(ReturnRequestFilterDto filter);
        Task EditReturnRequestAsync(int id, int userId, ReturnRequestCreateDto dto);
        Task CancelReturnRequestAsync(int id, int userId);
        Task SaveDraftAsync(int userId, ReturnRequestCreateDto dto);
        Task ChangeDraftToPendingAsync(int id, int userId);
    }
}
