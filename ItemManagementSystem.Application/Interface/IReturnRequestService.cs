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
    }
}
