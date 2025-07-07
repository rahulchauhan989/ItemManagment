using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;

namespace ItemManagementSystem.Application.Implementation
{
    public class ItemModelService : IItemModelService
    {
        private readonly IRepository<ItemModel> _repo;
        private readonly IMapper _mapper;

        public ItemModelService(IRepository<ItemModel> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ItemModelDto> CreateAsync(ItemModelDto dto)
        {
            var entity = _mapper.Map<ItemModel>(dto);
            entity.Quantity = 0; // always 0 on create
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ItemModelDto>(created);
        }

        public async Task<ItemModelDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ItemModelDto>(entity);
        }

        public async Task<IEnumerable<ItemModelDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ItemModelDto>>(entities);
        }

        public async Task UpdateAsync(int id, ItemModelDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException("ItemModel not found.");

            // Do not allow updating quantity from here
            var existingQuantity = entity.Quantity;
            _mapper.Map(dto, entity);
            entity.Quantity = existingQuantity;

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException("ItemModel not found.");

            await _repo.DeleteAsync(entity);
        }
    }
}