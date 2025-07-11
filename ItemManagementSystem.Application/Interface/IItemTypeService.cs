using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Application.Interface;

public interface IItemTypeService
{
    Task<ItemTypeCreateRequest> CreateAsync(ItemTypeCreateRequest dto,int userId);
    Task<ItemTypeDto?> GetByIdAsync(int id);
    Task<IEnumerable<ItemTypeDto>> GetAllAsync();
    Task<PagedResultDto<ItemTypeDto>> GetPagedItemTypesAsync(ItemTypeFilterDto filter);
    Task<ItemTypeCreateRequest> updateAsync(int id, ItemTypeCreateRequest dto, int userId);
    Task DeleteAsync(int id);
    int ExtractUserIdFromToken(string token);
}
