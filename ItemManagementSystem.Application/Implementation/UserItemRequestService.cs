using System.Linq.Expressions;
using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;

namespace ItemManagementSystem.Application.Services;

public class UserItemRequestService : IUserItemRequestService
{
    private readonly IRepository<ItemRequest> _requestRepo;
    private readonly IRepository<RequestItem> _requestItemRepo;
    private readonly IRepository<ItemModel> _itemModelRepo;
    private readonly IMapper _mapper;

    public UserItemRequestService(
        IRepository<ItemRequest> requestRepo,
        IRepository<RequestItem> requestItemRepo,
        IRepository<ItemModel> itemModelRepo,
        IMapper mapper)
    {
        _requestRepo = requestRepo;
        _requestItemRepo = requestItemRepo;
        _itemModelRepo = itemModelRepo;
        _mapper = mapper;
    }

    public async Task<ItemManagementSystem.Domain.Dto.Request.ItemRequestWithIdsResponseDto?> GetUserItemRequestByIdAsync(int id)
    {
        var entity = await _requestRepo.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            return null;

        var items = await _requestItemRepo.FindIncludingAsync(
            i => i.ItemRequestId == entity.Id && !i.IsDeleted,
            new System.Linq.Expressions.Expression<Func<RequestItem, object>>[] { i => i.ItemModel, i => i.ItemModel.ItemType });

        var response = new ItemManagementSystem.Domain.Dto.Request.ItemRequestWithIdsResponseDto
        {
            Id = entity.Id,
            RequestNumber = entity.RequestNumber,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            Items = items.Select(i => new ItemManagementSystem.Domain.Dto.Request.RequestItemWithIdsResponseDto
            {
                ItemModelId = i.ItemModelId,
                ItemTypeId = i.ItemModel?.ItemTypeId ?? 0,
                ItemModelName = i.ItemModel?.Name,
                ItemTypeName = i.ItemModel?.ItemType?.Name,
                Quantity = i.Quantity
            }).ToList()
        };

        return response;
    }

   
    public async Task SaveDraftAsync(int userId, CreateItemRequestDto dto)
    {
        var entity = new ItemRequest
        {
            UserId = userId,
            RequestNumber = GenerateRequestNumber(),
            Status = "Draft",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
        };
        await _requestRepo.AddAsync(entity);

        var requestItems = dto.Items
            .Select(dtoItem =>
            {
                var entityItem = _mapper.Map<RequestItem>(dtoItem);
                entityItem.ItemRequestId = entity.Id;
                entityItem.CreatedBy = userId;
                entityItem.CreatedAt = DateTime.UtcNow;
                return entityItem;
            }).ToList();

        foreach (var requestItem in requestItems)
        {
            await _requestItemRepo.AddAsync(requestItem);
        }
    }

    public async Task ChangeDraftToPendingAsync(int requestId, int userId)
    {
        var entity = await _requestRepo.GetByIdAsync(requestId);
        if (entity == null || entity.IsDeleted)
            throw new NullObjectException(AppMessages.ItemRequestNotFound);

        if (entity.UserId != userId)
            throw new CustomException(AppMessages.cannotEditOtherRequest);

        if (entity.Status != "Draft")
            throw new CustomException(AppMessages.OnlyDraftChangeToPending);

        entity.Status = "Pending";
        entity.UpdatedAt = DateTime.UtcNow;
        entity.ModifiedBy = userId;
        await _requestRepo.UpdateAsync(entity);
    }

        public async Task<ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto> CreateRequestAsync(int userId, CreateItemRequestDto dto)
        {
            foreach (var reqItem in dto.Items)
            {
                var item = await _itemModelRepo.GetByIdAsync(reqItem.ItemModelId);
                if (item == null || item.IsDeleted)
                    throw new CustomException(AppMessages.ItemModelNotFound);
                if (reqItem.Quantity > item.Quantity)
                    throw new CustomException($"Requested quantity for item {item.Name} exceeds available stock.");
            }

            var entity = new ItemRequest
            {
                UserId = userId,
                RequestNumber = GenerateRequestNumber(),
                Status = "Pending",
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
            };
            await _requestRepo.AddAsync(entity);

            var requestItems = dto.Items
                .Select(dtoItem =>
                {
                    var entityItem = new RequestItem
                    {
                        ItemRequestId = entity.Id,
                        ItemModelId = dtoItem.ItemModelId,
                        Quantity = dtoItem.Quantity,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    return entityItem;
                }).ToList();

            foreach (var requestItem in requestItems)
            {
                await _requestItemRepo.AddAsync(requestItem);
            }

            return new ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto
            {
                Id = entity.Id,
                RequestNumber = entity.RequestNumber,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                Items = dto.Items.Select(item => new ItemManagementSystem.Domain.Dto.Request.RequestItemResponseDto
                {
                    Quantity = item.Quantity,
                    ItemModelName = null,
                    ItemTypeName = null
                }).ToList()
            };
        }
 
    public async Task<PagedResultDto<ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto>> GetRequestsByUserPagedAsync(int userId, Domain.Dto.Request.ItemRequestFilterDto filter)
    {
        var filterProperties = new Dictionary<string, string?>();
        filterProperties.Add("UserId", userId.ToString());
        if (!string.IsNullOrEmpty(filter.Status))
        {
            filterProperties.Add("Status", filter.Status);
        }
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            filterProperties.Add("RequestNumber", filter.SearchTerm);
        }

        var pagedResult = await _requestRepo.GetPagedWithMultipleFiltersAndSortAsync(
            filterProperties,
            filter.SortBy,
            filter.SortDirection,
            filter.Page,
            filter.PageSize);

        var resultItems = new List<ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto>();
        foreach (var r in pagedResult.Items)
        {
            var items = await _requestItemRepo.FindIncludingAsync(
                i => i.ItemRequestId == r.Id && !i.IsDeleted,
                new System.Linq.Expressions.Expression<Func<RequestItem, object>>[] { i => i.ItemModel, i => i.ItemModel.ItemType });

            resultItems.Add(new ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto
            {
                Id = r.Id,
                RequestNumber = r.RequestNumber!,
                Status = r.Status!,
                CreatedAt = r.CreatedAt,
                Items = items.Select(i => new ItemManagementSystem.Domain.Dto.Request.RequestItemResponseDto
                {
                    Quantity = i.Quantity,
                    ItemModelName = i.ItemModel?.Name,
                    ItemTypeName = i.ItemModel?.ItemType?.Name
                }).ToList()
            });
        }

        return new PagedResultDto<ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto>
        {
            Items = resultItems,
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<bool> ChangeStatusAsync(int requestId, int userId)
    {
        var entity = await _requestRepo.GetByIdAsync(requestId);
        if (entity == null || entity.IsDeleted) return false;

        if (entity.CreatedBy != userId)
            throw new CustomException(AppMessages.cannotchangeOtherPersonstatus);

        entity.Status = "Cancelled";
        entity.UpdatedAt = DateTime.UtcNow;
        entity.ModifiedBy = userId;
        await _requestRepo.UpdateAsync(entity);
        return true;
    }

    private static string GenerateRequestNumber()
    {
        return $"REQ-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
    }

    private static HashSet<string> AllowedStatuses()
    {
        return new HashSet<string> { "Draft", "Pending", "Approved", "Cancelled", "Rejected" };
    }
}
