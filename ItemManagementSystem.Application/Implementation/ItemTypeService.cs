using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace ItemManagementSystem.Application.Implementation
{
    public class ItemTypeService : IItemTypeService
    {
        private readonly IRepository<ItemType> _repo;
        private readonly IMapper _mapper;

        private readonly IRepository<ItemModel> _itemmodel;

        public ItemTypeService(IRepository<ItemType> repo, IMapper mapper, IRepository<ItemModel> itemmodel)
        {
            _itemmodel = itemmodel;
            _repo = repo;
            _mapper = mapper;
        }

        public int ExtractUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new NullObjectException(AppMessages.NullToken);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new NullObjectException(AppMessages.UserIdNotFound);
            }

            return userId;
        }

        public async Task<ItemTypeCreateRequest> CreateAsync(ItemTypeCreateRequest dto, int userId)
        {
            if (dto == null)
                throw new NullObjectException(AppMessages.NullItemTypeRequest);

            var exists = (await _repo.FindAsync(
             it => it.Name.ToLower() == dto.Name.ToLower()
         )).Any();

            if (exists)
                throw new AlreadyExistsException(AppMessages.ItemTypeAlreadyExists);

            var entity = _mapper.Map<ItemType>(dto);
            entity.CreatedBy = userId;

            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ItemTypeCreateRequest>(created);
        }

        public async Task<ItemTypeResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.ItemTypeNotFound);

            if (entity.IsDeleted)
                throw new NullObjectException(AppMessages.ItemTypeNotFound);
            return _mapper.Map<ItemTypeResponseDto>(entity);
        }

        public async Task<IEnumerable<ItemTypeDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();

            var filtered = entities
                .Where(e => !e.IsDeleted)
                .ToList();

            return _mapper.Map<IEnumerable<ItemTypeDto>>(filtered);
        }

        public async Task<PagedResultDto<ItemTypePagedDto>> GetPagedItemTypesAsync(ItemTypeFilterDto filter)
        {
            var filterProperties = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                filterProperties.Add("Name", filter.SearchTerm);
            }

            var pagedResult = await _repo.GetPagedWithMultipleFiltersAndSortAsync(
                filterProperties,
                string.IsNullOrEmpty(filter.SortBy) ? "Name" : filter.SortBy,
                filter.SortDirection,
                filter.Page,
                filter.PageSize);

            var mappedItems = _mapper.Map<IEnumerable<ItemTypePagedDto>>(pagedResult.Items);

            return new PagedResultDto<ItemTypePagedDto>
            {
                Items = mappedItems,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize
            };
        }


        public async Task<ItemTypeCreateRequest> updateAsync(int id, ItemTypeCreateRequest dto, int userId)
        {

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                throw new NullObjectException(AppMessages.ItemTypeNotFound);

            //if same name exist
            var exists = (await _repo.FindAsync(
                it => it.Name.ToLower() == dto.Name.ToLower() && it.Id != id
            )).Any();

            if (exists)
                throw new AlreadyExistsException(AppMessages.ItemTypeAlreadyExists);


            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ModifiedBy = userId;
            await _repo.UpdateAsync(entity);
            return _mapper.Map<ItemTypeCreateRequest>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.ItemTypeNotFound);

            var hasAssociatedModels = (await _repo.FindAsync(
                it => it.ItemModels.Any(im => im.IsDeleted == false)
            )).Any();
            if (hasAssociatedModels)
                throw new CustomException(AppMessages.ItemTypeHasAssociatedModels);

            await _repo.DeleteAsync(entity);
        }
    }
}