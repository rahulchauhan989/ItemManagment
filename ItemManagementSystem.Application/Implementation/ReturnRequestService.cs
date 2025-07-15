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

    public async Task<ReturnRequestDto> CreateReturnRequestAsync(int userId, ReturnRequestCreateDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new CustomException(AppMessages.InvalidReturnItems);

        // Get items user actually has
        var approvedUserRequests  = await _requestItemRepo.FindAsync(ri =>
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
            Items = dto.Items
        };
    }

    public async Task<PagedResultDto<ReturnRequestDto>> GetUserReturnRequestsAsync(int userId, ReturnRequestFilterDto filter)
    {
        Expression<Func<ReturnRequest, bool>> predicate = r =>
            r.UserId == userId &&
            !r.IsDeleted &&
            (string.IsNullOrEmpty(filter.Status) || r.Status == filter.Status) &&
            (string.IsNullOrEmpty(filter.SearchTerm) || r.ReturnRequestNumber!=null && r.ReturnRequestNumber.Contains(filter.SearchTerm));

        Func<IQueryable<ReturnRequest>, IOrderedQueryable<ReturnRequest>> orderBy = filter.SortBy?.ToLower() switch
        {
            "status" => q => filter.SortDirection == "desc" ? q.OrderByDescending(x => x.Status) : q.OrderBy(x => x.Status),
            _ => q => filter.SortDirection == "desc" ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt)
        };

        var paged = await _returnRequestRepo.GetPagedAsync(predicate, orderBy, filter.Page, filter.PageSize);

        var result = new List<ReturnRequestDto>();

        foreach (var entity in paged.Items)
        {
            var items = await _returnRequestItemRepo.FindAsync(i => i.ReturnRequestId == entity.Id && !i.IsDeleted);
            var itemDtos = items.Select(i => new ReturnRequestItemDto
            {
                ItemModelId = i.ItemModelId,
                Quantity = i.Quantity
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
            throw new CustomException("Invalid status");

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

    public async Task<PagedResultDto<ReturnRequestDto>> GetAllReturnRequestsAsync(ReturnRequestFilterDto filter)
    {
        Expression<Func<ReturnRequest, bool>> predicate = r =>
            !r.IsDeleted &&
            (string.IsNullOrEmpty(filter.Status) || r.Status == filter.Status) &&
            (string.IsNullOrEmpty(filter.SearchTerm) || r.ReturnRequestNumber != null && r.ReturnRequestNumber.Contains(filter.SearchTerm));

        Func<IQueryable<ReturnRequest>, IOrderedQueryable<ReturnRequest>> orderBy = filter.SortBy?.ToLower() switch
        {
            "status" => q => filter.SortDirection == "desc" ? q.OrderByDescending(x => x.Status) : q.OrderBy(x => x.Status),
            _ => q => filter.SortDirection == "desc" ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt)
        };

        var paged = await _returnRequestRepo.GetPagedAsync(predicate, orderBy, filter.Page, filter.PageSize);

        var result = new List<ReturnRequestDto>();

        foreach (var entity in paged.Items)
        {
            var items = await _returnRequestItemRepo.FindAsync(i => i.ReturnRequestId == entity.Id && !i.IsDeleted);
            var itemDtos = items.Select(i => new ReturnRequestItemDto
            {
                ItemModelId = i.ItemModelId,
                Quantity = i.Quantity
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
}

