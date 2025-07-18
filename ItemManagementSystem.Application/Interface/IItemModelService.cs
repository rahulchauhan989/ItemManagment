using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Application.Interface;

public interface IItemModelService
{
    Task<ItemModelDto> CreateAsync(ItemModelDto dto);
    Task<ItemModelCreateDto> CreateAsync(ItemModelCreateDto dto, int userId);
    Task<ItemModelResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<ItemModelDto>> GetAllAsync();
    Task<PagedResultDto<ItemModelResponseDto>> GetPagedAsync(ItemModelFilterDto filter);
    Task<ItemModelDto> UpdateAsync(int id, ItemModelDto dto);
    Task<ItemModelCreateDto> UpdateAsync(int id, ItemModelCreateDto dto, int userId);
    Task DeleteAsync(int id);
}

