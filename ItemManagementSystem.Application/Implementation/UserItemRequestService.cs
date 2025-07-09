using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Infrastructure.Interface;

namespace ItemManagementSystem.Application.Services;

public class UserItemRequestService : IUserItemRequestService
{
    private readonly IRepository<ItemRequest> _requestRepo;
    private readonly IRepository<RequestItem> _requestItemRepo;
    private readonly IRepository<ItemModel> _itemModelRepo;

    public UserItemRequestService(
        IRepository<ItemRequest> requestRepo,
        IRepository<RequestItem> requestItemRepo,
        IRepository<ItemModel> itemModelRepo)
    {
        _requestRepo = requestRepo;
        _requestItemRepo = requestItemRepo;
        _itemModelRepo = itemModelRepo;
    }

    public async Task<ItemRequestResponseDto> CreateRequestAsync(int userId, CreateItemRequestDto dto)
    {
        foreach (var reqItem in dto.Items)
        {
            var item = await _itemModelRepo.GetByIdAsync(reqItem.ItemModelId);
            if (item == null || item.IsDeleted)
                throw new InvalidOperationException($"Item with Id {reqItem.ItemModelId} does not exist.");
            if (reqItem.Quantity > item.Quantity)
                throw new InvalidOperationException($"Requested quantity for item {item.Name} exceeds available stock.");
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
        // await _requestRepo.SaveChangesAsync();

        // Add request items
        foreach (var reqItem in dto.Items)
        {
            var requestItem = new RequestItem
            {
                RequestId = entity.Id,
                ItemModelId = reqItem.ItemModelId,
                Quantity = reqItem.Quantity,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
            };
            await _requestItemRepo.AddAsync(requestItem);
        }
        // await _requestItemRepo.SaveChangesAsync();

        var response = new ItemRequestResponseDto
        {
            Id = entity.Id,
            RequestNumber = entity.RequestNumber!,
            Status = entity.Status!,
            CreatedAt = entity.CreatedAt,
            Items = dto.Items
        };
        return response;
    }

    public async Task<List<ItemRequestResponseDto>> GetRequestsByUserAsync(int userId)
    {
        var requests = await _requestRepo.FindAsync(r => r.UserId == userId && !r.IsDeleted);
        var result = new List<ItemRequestResponseDto>();
        foreach (var r in requests.OrderByDescending(x => x.CreatedAt))
        {
            var items = await _requestItemRepo.FindAsync(i => i.RequestId == r.Id && !i.IsDeleted);
            result.Add(new ItemRequestResponseDto
            {
                Id = r.Id,
                RequestNumber = r.RequestNumber!,
                Status = r.Status!,
                CreatedAt = r.CreatedAt,
                Items = items.Select(i => new RequestItemDto
                {
                    ItemModelId = i.ItemModelId,
                    Quantity = i.Quantity
                }).ToList()
            });
        }
        return result;
    }

    public async Task<bool> ChangeStatusAsync(int requestId, string newStatus, int userId)
    {
        var entity = await _requestRepo.GetByIdAsync(requestId);
        if (entity == null || entity.IsDeleted) return false;

        if (!AllowedStatuses().Contains(newStatus)) return false;

        entity.Status = newStatus;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.ModifiedBy = userId;
        await _requestRepo.UpdateAsync(entity);
        // await _requestRepo.SaveChangesAsync();
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