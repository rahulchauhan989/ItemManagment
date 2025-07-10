using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;

namespace ItemManagementSystem.Application.Implementation
{
    public class ItemModelService : IItemModelService
    {
        private readonly IRepository<ItemModel> _repo;
        private readonly IRepository<RequestItem> _requestItemRepo;
        private readonly IRepository<ItemRequest> _itemRequestRepo;
        private readonly IMapper _mapper;

        public ItemModelService(IRepository<ItemModel> repo, IMapper mapper,
            IRepository<RequestItem> requestItemRepo, IRepository<ItemRequest> itemRequestRepo)
        {
            _requestItemRepo = requestItemRepo;
            _repo = repo;
            _mapper = mapper;
            _itemRequestRepo = itemRequestRepo;
        }

        public async Task<ItemModelDto> CreateAsync(ItemModelDto dto)
        {
            dto.Id = 0;
            dto.modifiedBy = null;
            //if ItemModal with same name already exists in same ItemType, throw exception
            var exists = (await _repo.FindAsync(
                it => it.Name.ToLower() == dto.Name.ToLower() && it.ItemTypeId == dto.ItemTypeId
            )).Any();
            if (exists)
                throw new AlreadyExistsException(AppMessages.ItemModelAlreadyExists);

            var entity = _mapper.Map<ItemModel>(dto);
            entity.Quantity = 0;
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ItemModelDto>(created);
        }

        public async Task<ItemModelCreateDto> CreateAsync(ItemModelCreateDto dto, int userId)
        {

            //if ItemModal with same name already exists in same ItemType, throw exception
            var exists = (await _repo.FindAsync(
                it => it.Name.ToLower() == dto.Name.ToLower() && it.ItemTypeId == dto.ItemTypeId
            )).Any();
            if (exists)
                throw new AlreadyExistsException(AppMessages.ItemModelAlreadyExists);

            //if itemtype Id exist or not
            bool isItemTypeIdExist = (await _repo.FindAsync(
                it => it.ItemTypeId == dto.ItemTypeId
            )).Any();
            if (!isItemTypeIdExist)
                throw new NullObjectException(AppMessages.ItemTypeNotFound);

            var entity = _mapper.Map<ItemModel>(dto);
            entity.CreatedBy = userId;
            entity.Quantity = 0;

            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ItemModelCreateDto>(created);
        }

        public async Task<ItemModelDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.ItemModelNotFound);
            return _mapper.Map<ItemModelDto>(entity);
        }

        public async Task<IEnumerable<ItemModelDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ItemModelDto>>(entities);
        }

        public async Task<ItemModelDto> UpdateAsync(int id, ItemModelDto dto)
        {
            dto.Id = id;
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.ItemModelNotFound);

            // if ItemModel with same name already exists in same ItemType, throw exception
            var exists = (await _repo.FindAsync(
                it => it.Name.ToLower() == dto.Name.ToLower() && it.ItemTypeId == dto.ItemTypeId && it.Id != id
            )).Any();
            if (exists)
                throw new AlreadyExistsException(AppMessages.ItemModelAlreadyExists);

            dto.createdBy = entity.CreatedBy;

            // Do not allow updating quantity from here
            var existingQuantity = entity.Quantity;
            _mapper.Map(dto, entity);
            entity.Quantity = existingQuantity;

            await _repo.UpdateAsync(entity);
            return _mapper.Map<ItemModelDto>(entity);
        }

        public async Task<ItemModelCreateDto> UpdateAsync(int id, ItemModelCreateDto dto, int userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.ItemModelNotFound);

            // if ItemModel with same name already exists in same ItemType, throw exception
            var exists = (await _repo.FindAsync(
                it => it.Name.ToLower() == dto.Name.ToLower() && it.ItemTypeId == dto.ItemTypeId && it.Id != id
            )).Any();
            if (exists)
                throw new AlreadyExistsException(AppMessages.ItemModelAlreadyExists);

            //if itemtype Id exist or not
            bool isItemTypeIdExist = (await _repo.FindAsync(
                it => it.ItemTypeId == dto.ItemTypeId
            )).Any();
            if (!isItemTypeIdExist)
                throw new NullObjectException(AppMessages.ItemTypeNotFound);


            // Do not allow updating quantity from here
            var existingQuantity = entity.Quantity;
            _mapper.Map(dto, entity);
            entity.Quantity = existingQuantity;
            entity.ModifiedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            return _mapper.Map<ItemModelCreateDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            // 1. Get the ItemModel entity
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.ItemModelNotFound);

            // 2. Check for pending requests referencing this model
            // If using repository pattern:
            var requestItems = await _requestItemRepo.FindAsync(ri => ri.ItemModelId == id);

            // Now check if any of their ItemRequest.Status == "pending"
            foreach (var requestItem in requestItems)
            {
                var itemRequest = await _itemRequestRepo.GetByIdAsync(requestItem.RequestId);
                if (itemRequest != null && itemRequest.Status == "Pending")
                {
                    throw new CustomException(AppMessages.ItemModelHasAssociatedRequests);
                }
            }

            await _repo.DeleteAsync(entity);
        }
    }
}