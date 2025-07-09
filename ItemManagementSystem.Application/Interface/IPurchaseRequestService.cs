using ItemManagementSystem.Domain.Dto;

namespace ItemManagementSystem.Application.Interface;

public interface IPurchaseRequestService
{
    Task<PurchaseRequestDto> CreateAsync(PurchaseRequestDto dto);
    Task<PurchaseRequestDto?> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseRequestDto>> GetAllAsync(PurchaseRequestFilterDto filter);
}
