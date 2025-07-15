using System.Security.Cryptography;
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
    public class PurchaseRequestService : IPurchaseRequestService
    {
        private readonly IRepository<PurchaseRequest> _purchaseRepo;
        private readonly IRepository<ItemModel> _itemModelRepo;
        private readonly IRepository<ItemType> _itemTypeRepo;
        private readonly IRepository<PurchaseRequestItem> _purchaseItemRepo;
        private readonly IMapper _mapper;

        public PurchaseRequestService(
            IRepository<PurchaseRequest> purchaseRepo,
            IRepository<ItemModel> itemModelRepo,
            IRepository<PurchaseRequestItem> purchaseItemRepo,
            IRepository<ItemType> itemTypeRepo,
            IMapper mapper)
        {
            _purchaseRepo = purchaseRepo;
            _itemModelRepo = itemModelRepo;
            _purchaseItemRepo = purchaseItemRepo;
            _itemTypeRepo = itemTypeRepo;
            _mapper = mapper;
        }

        public async Task<PurchaseRequestDto> CreateAsync(PurchaseRequestDto dto)
        {
            dto.Id = 0;
            dto.InvoiceNumber = GenerateInvoiceNumber();
            dto.UpdatedAt = null;
            dto.ModifiedBy = null;

            if (dto.CreatedBy == null)
            {
                throw new NullObjectException(AppMessages.NullCreatedBy);
            }

            // var itemType = await _itemTypeRepository.GetByIdAsync(dto.ItemTypeId);
            // if (itemType == null)
            //     throw new CustomException($"ItemType with Id {dto.ItemTypeId} does not exist.");

            var purchaseEntity = _mapper.Map<PurchaseRequest>(dto);
            var createdPurchase = await _purchaseRepo.AddAsync(purchaseEntity);

            var savedItems = new List<PurchaseRequestItemDto>();
            foreach (var itemDto in dto.Items)
            {
                var itemModel = await _itemModelRepo.GetByIdAsync(itemDto.ItemModelId);
                if (itemModel == null || itemModel.IsDeleted)
                    throw new NullObjectException($"ItemModel with id {itemDto.ItemModelId} not found.");

                itemModel.Quantity += itemDto.Quantity;
                await _itemModelRepo.UpdateAsync(itemModel);

                var purchaseItem = new PurchaseRequestItem
                {
                    PurchaseRequestId = createdPurchase.Id,
                    ItemModelId = itemDto.ItemModelId,
                    Quantity = itemDto.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = (int)dto.CreatedBy,
                    IsDeleted = false
                };
                await _purchaseItemRepo.AddAsync(purchaseItem);

                savedItems.Add(new PurchaseRequestItemDto
                {
                    ItemModelId = itemDto.ItemModelId,
                    Quantity = itemDto.Quantity
                });
            }

            var resultDto = _mapper.Map<PurchaseRequestDto>(createdPurchase);
            resultDto.Items = savedItems;
            return resultDto;
        }

        public async Task<PurchaseRequestDto> CreateAsync(PurchaseRequestCreateDto dto, int userId)
        {


            if (dto.Items == null || dto.Items.Count == 0)
                throw new NullObjectException(AppMessages.PurchaseRequestItemsCannotBeEmpty);

            foreach (var itemdto in dto.Items)
            {
                var itemModel = await _itemModelRepo.GetByIdAsync(itemdto.ItemModelId);
                if (itemModel == null || itemModel.IsDeleted)
                    throw new NullObjectException(AppMessages.ItemModelsNotFound);
            }

            var purchaseEntity = _mapper.Map<PurchaseRequest>(dto);
            purchaseEntity.Date = DateTime.UtcNow;
            purchaseEntity.InvoiceNumber = GenerateInvoiceNumber();
            purchaseEntity.CreatedBy = userId;
            var createdPurchase = await _purchaseRepo.AddAsync(purchaseEntity);

            var savedItems = new List<PurchaseRequestItemDto>();
            foreach (var itemDto in dto.Items)
            {
                var itemModel = await _itemModelRepo.GetByIdAsync(itemDto.ItemModelId);
                if (itemModel == null || itemModel.IsDeleted)
                    throw new NullObjectException(AppMessages.ItemModelsNotFound);

                itemModel.Quantity += itemDto.Quantity;
                await _itemModelRepo.UpdateAsync(itemModel);

                // AutoMapper for mapping
                var purchaseItem = _mapper.Map<PurchaseRequestItem>(itemDto);
                purchaseItem.PurchaseRequestId = createdPurchase.Id;
                purchaseItem.CreatedBy = userId;
                purchaseItem.IsDeleted = false;

                await _purchaseItemRepo.AddAsync(purchaseItem);

                var purchaseItemDto = _mapper.Map<PurchaseRequestItemDto>(purchaseItem);
                savedItems.Add(purchaseItemDto);
            }

            var resultDto = _mapper.Map<PurchaseRequestDto>(createdPurchase);
            resultDto.Items = savedItems;
            return resultDto;
        }

        public async Task<PurchaseRequestDto?> GetByIdAsync(int id)
        {
            var entity = await _purchaseRepo.GetByIdAsync(id);
            if (entity == null)
                throw new NullObjectException(AppMessages.PurchaseRequestNotFound);

            var dto = _mapper.Map<PurchaseRequestDto>(entity);

            // 1. Fetch related PurchaseRequestItems
            var items = await _purchaseItemRepo.FindAsync(x => x.PurchaseRequestId == entity.Id);

            // 2. For each item, fetch ItemModel and ItemType
            var itemDtos = new List<PurchaseRequestItemDto>();
            foreach (var item in items)
            {
                var itemModel = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                string? itemModelName = itemModel?.Name;
                int itemTypeId = itemModel?.ItemTypeId ?? 0;
                string? itemTypeName = null;

                if (itemModel != null)
                {
                    var itemType = await _itemTypeRepo.GetByIdAsync(itemModel.ItemTypeId);
                    itemTypeName = itemType?.Name;
                }

                itemDtos.Add(new PurchaseRequestItemDto
                {
                    ItemModelId = item.ItemModelId,
                    Name = itemModelName,
                    Quantity = item.Quantity,
                    ItemTypeId = itemTypeId,
                    ItemType = itemTypeName
                });
            }

            dto.Items = itemDtos;
            return dto;
        }

        public async Task<IEnumerable<PurchaseRequestDto>> GetAllAsync(PurchaseRequestFilterDto filter)
        {
            var query = await _purchaseRepo.GetAllAsync();

            // Filtering
            if (filter.CreatedBy.HasValue)
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);

            if (filter.Date.HasValue)
                query = query.Where(x => x.Date.Date == filter.Date.Value.Date);

            // Sorting
            if (filter.SortBy == "Date")
                query = filter.SortDirection == "desc" ? query.OrderByDescending(x => x.Date) : query.OrderBy(x => x.Date);
            else
                query = query.OrderByDescending(x => x.CreatedAt);

            // Pagination
            int skip = (filter.Page - 1) * filter.PageSize;
            query = query.Skip(skip).Take(filter.PageSize);

            var purchaseList = query.ToList();

            var result = new List<PurchaseRequestDto>();
            foreach (var entity in purchaseList)
            {
                var dto = _mapper.Map<PurchaseRequestDto>(entity);

                var items = await _purchaseItemRepo.FindAsync(x => x.PurchaseRequestId == entity.Id);
                var itemDtos = new List<PurchaseRequestItemDto>();
                foreach (var item in items)
                {
                    var itemModel = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                    string? itemModelName = itemModel?.Name;
                    int itemTypeId = itemModel?.ItemTypeId ?? 0;
                    string? itemTypeName = null;

                    if (itemModel != null)
                    {
                        var itemType = await _itemTypeRepo.GetByIdAsync(itemModel.ItemTypeId);
                        itemTypeName = itemType?.Name;
                    }

                    itemDtos.Add(new PurchaseRequestItemDto
                    {
                        ItemModelId = item.ItemModelId,
                        Name = itemModelName,
                        Quantity = item.Quantity,
                        ItemTypeId = itemTypeId,
                        ItemType = itemTypeName
                    });
                }
                dto.Items = itemDtos;
                result.Add(dto);
            }
            return result;
        }

        private string GenerateInvoiceNumber()
        {

            return $"PR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}";
        }
    }
}