using ItemManagementSystem.Domain.Dto;

namespace ItemManagementSystem.Application.Interface;

public interface IItemModelService
{
    Task<ItemModelDto> CreateAsync(ItemModelDto dto);
    Task<ItemModelDto?> GetByIdAsync(int id);
    Task<IEnumerable<ItemModelDto>> GetAllAsync();
    Task UpdateAsync(int id, ItemModelDto dto);
    Task DeleteAsync(int id);
}

