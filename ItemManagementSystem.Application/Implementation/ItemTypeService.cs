using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;

namespace ItemManagementSystem.Application.Implementation
{
    public class ItemTypeService : IItemTypeService
    {
        private readonly IRepository<ItemType> _repo;
        private readonly IMapper _mapper;

        public ItemTypeService(IRepository<ItemType> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public int ExtractUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID not found in token.");
            }

            return userId;
        }

        public async Task<ItemTypeDto> CreateAsync(ItemTypeDto dto)
        {

            var entity = _mapper.Map<ItemType>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ItemTypeDto>(created);
        }

        public async Task<ItemTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ItemTypeDto>(entity);
        }

        public async Task<IEnumerable<ItemTypeDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ItemTypeDto>>(entities);
        }

        public async Task UpdateAsync(int id, ItemTypeDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException("ItemType not found.");

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException("ItemType not found.");

            await _repo.DeleteAsync(entity);
        }
    }
}