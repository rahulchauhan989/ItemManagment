using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Application.Interface;

public interface IPurchaseRequestService
{
    Task<PurchaseRequestDto> CreateAsync(PurchaseRequestDto dto);

    Task<PurchaseRequestDto> CreateAsync(PurchaseRequestCreateDto dto, int userId);
    Task<PurchaseRequestResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseRequestResponseDto>> GetAllAsync(PurchaseRequestFilterDto filter);
}
