using System.Linq.Expressions;
using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto.Request;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;

namespace ItemManagementSystem.Application.Implementation;

public class ReturnRequestService : IReturnRequestService
{
    private readonly IRepository<ReturnRequest> _returnRequestRepo;
    private readonly IRepository<ReturnRequestItem> _returnRequestItemRepo;
    private readonly IRepository<ItemModel> _itemModelRepo;

    private readonly IRepository<RequestItem> _requestItemRepo;
    private readonly IMapper _mapper;
    private readonly IRepository<User> _userRepo;

    public ReturnRequestService(
        IRepository<ReturnRequest> returnRequestRepo,
        IRepository<ReturnRequestItem> returnRequestItemRepo,
        IRepository<ItemModel> itemModelRepo,
        IRepository<ItemRequest> itemRequestRepo,
        IRepository<RequestItem> requestItemRepo,
        IMapper mapper,
        IRepository<User> userRepo)
    {
        _returnRequestRepo = returnRequestRepo;
        _returnRequestItemRepo = returnRequestItemRepo;
        _itemModelRepo = itemModelRepo;
        _requestItemRepo = requestItemRepo;
        _mapper = mapper;
        _userRepo = userRepo;
    }

    public async Task<PagedResultDto<ReturnRequestDto>> GetAllReturnRequestsAsync(ReturnRequestFilterDto filter)
    {
        var filterProperties = new Dictionary<string, string?>();
        if (!string.IsNullOrEmpty(filter.Status))
        {
            filterProperties.Add("Status", filter.Status);
        }
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            filterProperties.Add("ReturnRequestNumber", filter.SearchTerm);
        }

        var paged = await _returnRequestRepo.GetPagedWithMultipleFiltersAndSortAsync(
            filterProperties,
            filter.SortBy,
            filter.SortDirection,
            filter.Page,
            filter.PageSize);

        var result = new List<ReturnRequestDto>();

        foreach (var entity in paged.Items)
        {
            var items = await _returnRequestItemRepo.FindIncludingAsync(
                i => i.ReturnRequestId == entity.Id && !i.IsDeleted,
                new System.Linq.Expressions.Expression<Func<ReturnRequestItem, object>>[] { i => i.ItemModel, i => i.ItemModel.ItemType });

            var itemDtos = items.Select(i => new ReturnRequestItemDto
            {
                ItemModelId = i.ItemModelId,
                Quantity = i.Quantity,
                ItemModelName = i.ItemModel?.Name,
                ItemTypeId = i.ItemModel?.ItemTypeId ?? 0,
                ItemTypeName = i.ItemModel?.ItemType?.Name
            }).ToList();

            var user = (await _userRepo.FindAsync(u => u.Id == entity.UserId && u.Active)).FirstOrDefault();
            if (user == null)
                throw new NullObjectException(AppMessages.UserNotFound);

            result.Add(new ReturnRequestDto
            {
                Id = entity.Id,
                ReturnRequestNumber = entity.ReturnRequestNumber!,
                Status = entity.Status!,
                CreatedAt = entity.CreatedAt,
                UserId = entity.UserId,
                UserName = user.Name,
                Items = itemDtos
            });
        }

        return new PagedResultDto<ReturnRequestDto>
        {
            Items = result,
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    private static string GenerateReturnRequestNumber()
    {
        return $"RR-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}";
    }

    public async Task<ReturnRequestDto> CreateReturnRequestAsync(int userId, ReturnRequestCreateDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new CustomException(AppMessages.InvalidReturnItems);

        // Get items user actually has
        var approvedUserRequests = await _requestItemRepo.FindAsync(ri =>
            ri.CreatedBy == userId && !ri.IsDeleted && ri.ItemRequest.Status == "Approved");

        var itemAvailability = approvedUserRequests
            .GroupBy(i => i.ItemModelId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        foreach (var item in dto.Items)
        {
            if (!itemAvailability.ContainsKey(item.ItemModelId) || item.Quantity > itemAvailability[item.ItemModelId])
                throw new CustomException($"User doesn't own enough of item ID: {item.ItemModelId}");
        }

        var returnEntity = new ReturnRequest
        {
            UserId = userId,
            ReturnRequestNumber = GenerateReturnRequestNumber(),
            Status = "Pending",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _returnRequestRepo.AddAsync(returnEntity);

        foreach (var item in dto.Items)
        {
            var entityItem = new ReturnRequestItem
            {
                ReturnRequestId = returnEntity.Id,
                ItemModelId = item.ItemModelId,
                Quantity = item.Quantity,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _returnRequestItemRepo.AddAsync(entityItem);
        }

        return new ReturnRequestDto
        {
            Id = returnEntity.Id,
            ReturnRequestNumber = returnEntity.ReturnRequestNumber,
            Status = returnEntity.Status,
            CreatedAt = returnEntity.CreatedAt,
            Items = dto.Items.Select(i => new ReturnRequestItemDto
            {
                ItemModelId = i.ItemModelId,
                ItemModelName = string.Empty,
                ItemTypeId = 0,
                ItemTypeName = string.Empty,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task SaveDraftAsync(int userId, ReturnRequestCreateDto dto)
    {
        var returnEntity = new ReturnRequest
        {
            UserId = userId,
            ReturnRequestNumber = GenerateReturnRequestNumber(),
            Status = "Draft",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _returnRequestRepo.AddAsync(returnEntity);

        foreach (var item in dto.Items)
        {
            var entityItem = new ReturnRequestItem
            {
                ReturnRequestId = returnEntity.Id,
                ItemModelId = item.ItemModelId,
                Quantity = item.Quantity,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _returnRequestItemRepo.AddAsync(entityItem);
        }
    }

    public async Task ChangeDraftToPendingAsync(int id, int userId)
    {
        var request = await _returnRequestRepo.GetByIdAsync(id);
        if (request == null || request.IsDeleted)
            throw new NullObjectException(AppMessages.ReturnRequestNotFound);

        if (request.UserId != userId)
            throw new CustomException(AppMessages.cannotEditOtherRequest);

        if (request.Status != "Draft")
            throw new CustomException(AppMessages.OnlyDraftChangeToPending);

        request.Status = "Pending";
        request.UpdatedAt = DateTime.UtcNow;
        request.ModifiedBy = userId;
        await _returnRequestRepo.UpdateAsync(request);
    }

    public async Task<PagedResultDto<ReturnRequestDto>> GetUserReturnRequestsAsync(int userId, ReturnRequestFilterDto filter)
    {
        var filterProperties = new Dictionary<string, string?>();
        filterProperties.Add("UserId", userId.ToString());
        if (!string.IsNullOrEmpty(filter.Status))
        {
            filterProperties.Add("Status", filter.Status);
        }
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            filterProperties.Add("ReturnRequestNumber", filter.SearchTerm);
        }

        var paged = await _returnRequestRepo.GetPagedWithMultipleFiltersAndSortAsync(
            filterProperties,
            filter.SortBy,
            filter.SortDirection,
            filter.Page,
            filter.PageSize);

        var result = new List<ReturnRequestDto>();

        foreach (var entity in paged.Items)
        {
            var items = await _returnRequestItemRepo.FindIncludingAsync(
                i => i.ReturnRequestId == entity.Id && !i.IsDeleted,
                new System.Linq.Expressions.Expression<Func<ReturnRequestItem, object>>[] { i => i.ItemModel, i => i.ItemModel.ItemType });

            var itemDtos = items.Select(i => new ReturnRequestItemDto
            {
                ItemModelId = i.ItemModelId,
                Quantity = i.Quantity,
                ItemModelName = i.ItemModel?.Name,
                ItemTypeId = i.ItemModel?.ItemTypeId ?? 0,
                ItemTypeName = i.ItemModel?.ItemType?.Name
            }).ToList();

            result.Add(new ReturnRequestDto
            {
                Id = entity.Id,
                ReturnRequestNumber = entity.ReturnRequestNumber!,
                Status = entity.Status!,
                CreatedAt = entity.CreatedAt,
                Items = itemDtos
            });
        }

        return new PagedResultDto<ReturnRequestDto>
        {
            Items = result,
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task UpdateReturnRequestStatusAsync(int id, string status, string? comment, int userId)
    {
        var request = await _returnRequestRepo.GetByIdAsync(id);
        if (request == null || request.IsDeleted)
            throw new NullObjectException(AppMessages.ReturnRequestNotFound);

        var validStatuses = new[] { "Approved", "Rejected" };
        if (!validStatuses.Contains(status))
            throw new CustomException(AppMessages.InvalidRequest);

        if (request.Status == status)
            throw new CustomException($"Already in {status} status");

        var items = await _returnRequestItemRepo.FindAsync(i => i.ReturnRequestId == id && !i.IsDeleted);

        if (status == "Approved")
        {
            foreach (var item in items)
            {
                var model = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                if (model != null)
                {
                    model.Quantity += item.Quantity;
                    await _itemModelRepo.UpdateAsync(model);
                }
            }
        }

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;
        request.ModifiedBy = userId;
        // request.Comment = comment;

        await _returnRequestRepo.UpdateAsync(request);
    }

    public async Task EditReturnRequestAsync(int id, int userId, ReturnRequestCreateDto dto)
    {
        var request = await _returnRequestRepo.GetByIdAsync(id);
        if (request == null || request.IsDeleted)
            throw new NullObjectException(AppMessages.ReturnRequestNotFound);

        if (request.UserId != userId)
            throw new CustomException(AppMessages.cannotEditOtherRequest);

        if (request.Status != "Pending")
            throw new CustomException(AppMessages.OnlyPendingReqEditable);

        var approvedUserRequests = await _requestItemRepo.FindAsync(ri =>
            ri.CreatedBy == userId && !ri.IsDeleted && ri.ItemRequest.Status == "Approved");

        var itemAvailability = approvedUserRequests
            .GroupBy(i => i.ItemModelId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        foreach (var item in dto.Items)
        {
            if (!itemAvailability.ContainsKey(item.ItemModelId) || item.Quantity > itemAvailability[item.ItemModelId])
                throw new CustomException($"User doesn't own enough of item ID: {item.ItemModelId}");
        }

        var existingItems = await _returnRequestItemRepo.FindAsync(i => i.ReturnRequestId == id && !i.IsDeleted);


        var existingItemsDict = existingItems.ToDictionary(i => i.ItemModelId);

        foreach (var item in dto.Items)
        {
            if (existingItemsDict.TryGetValue(item.ItemModelId, out var existingItem))
            {
                existingItem.Quantity = item.Quantity;
                await _returnRequestItemRepo.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new ReturnRequestItem
                {
                    ReturnRequestId = id,
                    ItemModelId = item.ItemModelId,
                    Quantity = item.Quantity,
                    IsDeleted = false
                };
                await _returnRequestItemRepo.AddAsync(newItem);
            }
        }

        request.UpdatedAt = DateTime.UtcNow;
        request.ModifiedBy = userId;
        await _returnRequestRepo.UpdateAsync(request);
    }

    public async Task CancelReturnRequestAsync(int id, int userId)
    {
        var request = await _returnRequestRepo.GetByIdAsync(id);
        if (request == null || request.IsDeleted)
            throw new NullObjectException(AppMessages.ReturnRequestNotFound);

        if (request.UserId != userId)
            throw new CustomException(AppMessages.cannotCancelOtherRequest);

        if (request.Status != "Pending")
            throw new CustomException(AppMessages.OnlyPendingReqCancelable);

        request.Status = "Cancelled";
        request.UpdatedAt = DateTime.UtcNow;
        request.ModifiedBy = userId;
        await _returnRequestRepo.UpdateAsync(request);
    }
}
