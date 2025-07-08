using ItemManagementSystem.Domain.Dto;

namespace ItemManagementSystem.Application.Interface;

public interface IItemTypeService
{
    Task<ItemTypeDto> CreateAsync(ItemTypeDto dto);
    Task<ItemTypeDto?> GetByIdAsync(int id);
    Task<IEnumerable<ItemTypeDto>> GetAllAsync();
    Task<ItemTypeDto> UpdateAsync(int id, ItemTypeDto dto);
    Task DeleteAsync(int id);
    int ExtractUserIdFromToken(string token);
}
