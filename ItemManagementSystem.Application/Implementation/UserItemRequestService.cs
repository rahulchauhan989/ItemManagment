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

    // public async Task<ItemRequestResponseDto> CreateRequestAsync(int userId, CreateItemRequestDto dto)
    // {
    //     foreach (var reqItem in dto.Items)
    //     {
    //         var item = await _itemModelRepo.GetByIdAsync(reqItem.ItemModelId);
    //         if (item == null || item.IsDeleted)
    //             throw new CustomException(AppMessages.ItemModelNotFound);
    //         if (reqItem.Quantity > item.Quantity)
    //             throw new InvalidOperationException($"Requested quantity for item {item.Name} exceeds available stock.");
    //     }

    //     var entity = new ItemRequest
    //     {
    //         UserId = userId,
    //         RequestNumber = GenerateRequestNumber(),
    //         Status = "Pending",
    //         CreatedBy = userId,
    //         CreatedAt = DateTime.UtcNow,
    //     };
    //     await _requestRepo.AddAsync(entity);

    //     // Add request items
    //     foreach (var reqItem in dto.Items)
    //     {
    //         var requestItem = new RequestItem
    //         {
    //             RequestId = entity.Id,
    //             ItemModelId = reqItem.ItemModelId,
    //             Quantity = reqItem.Quantity,
    //             CreatedBy = userId,
    //             CreatedAt = DateTime.UtcNow,
    //         };
    //         await _requestItemRepo.AddAsync(requestItem);
    //     }

    //     var response = new ItemRequestResponseDto
    //     {
    //         Id = entity.Id,
    //         RequestNumber = entity.RequestNumber,
    //         Status = entity.Status,
    //         CreatedAt = entity.CreatedAt,
    //         Items = dto.Items
    //     };
    //     return response;
    // }

    public async Task<ItemRequestResponseDto> CreateRequestAsync(int userId, CreateItemRequestDto dto)
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

        //  Use AutoMapper to map Dto to Entities
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

        //  Prepare response: Only necessary fields
        return new ItemRequestResponseDto
        {
            Id = entity.Id,
            RequestNumber = entity.RequestNumber,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            Items = dto.Items.Select(item => new RequestItemDto
            {
                ItemModelId = item.ItemModelId,
                Quantity = item.Quantity
            }).ToList()
        };
    }
 
        public async Task<PagedResultDto<ItemRequestResponseDto>> GetRequestsByUserPagedAsync(int userId, Domain.Dto.Request.ItemRequestFilterDto filter)
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

            var resultItems = new List<ItemRequestResponseDto>();
            foreach (var r in pagedResult.Items)
            {
                var items = await _requestItemRepo.FindIncludingAsync(
                    i => i.ItemRequestId == r.Id && !i.IsDeleted,
                    new System.Linq.Expressions.Expression<Func<RequestItem, object>>[] { i => i.ItemModel, i => i.ItemModel.ItemType });

                resultItems.Add(new ItemRequestResponseDto
                {
                    Id = r.Id,
                    RequestNumber = r.RequestNumber!,
                    Status = r.Status!,
                    CreatedAt = r.CreatedAt,
                    Items = items.Select(i => new RequestItemDto
                    {
                        ItemModelId = i.ItemModelId,
                        Quantity = i.Quantity,
                        ItemModelName = i.ItemModel?.Name,
                        ItemTypeId = i.ItemModel?.ItemTypeId ?? 0,
                        ItemTypeName = i.ItemModel?.ItemType?.Name
                    }).ToList()
                });
            }

            return new PagedResultDto<ItemRequestResponseDto>
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

        //check requestId's createdBy field is userid
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