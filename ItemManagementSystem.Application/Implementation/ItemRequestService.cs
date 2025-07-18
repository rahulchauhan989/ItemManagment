using System.Linq.Expressions;
using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataContext;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using ItemRequestResponseDto = ItemManagementSystem.Domain.Dto.Request.ItemRequestResponseDto;
using RequestItemResponseDto = ItemManagementSystem.Domain.Dto.Request.RequestItemResponseDto;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;


namespace ItemManagementSystem.Application.Implementation
{
    public class ItemRequestService : IItemRequestService
    {
        private readonly IRepository<ItemRequest> _itemRequestRepo;
        private readonly IRepository<RequestItem> _requestItemRepo;
        private readonly IRepository<ItemModel> _itemModelRepo;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepo;

        public ItemRequestService(
            IRepository<ItemRequest> itemRequestRepo,
            IRepository<RequestItem> requestItemRepo,
            IRepository<ItemModel> itemModelRepo,
            IMapper mapper,
            IRepository<User> userRepo)
        {
            _itemRequestRepo = itemRequestRepo;
            _requestItemRepo = requestItemRepo;
            _itemModelRepo = itemModelRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }
        public async Task<PagedResultDto<ItemRequestResponseDto>> GetRequestsAsync(ItemsRequestFilterDto filter)
        {
            var filterProperties = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(filter.RequestNumber))
            {
                filterProperties.Add("RequestNumber", filter.RequestNumber);
            }
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                filterProperties.Add("User.Name", filter.UserName);
            }

            var paged = await _itemRequestRepo.GetPagedWithMultipleFiltersAndSortAsync(
                filterProperties,
                filter.SortBy,
                filter.SortDirection,
                filter.Page,
                filter.PageSize);

            var result = new List<ItemRequestResponseDto>();

            foreach (var entity in paged.Items)
            {
                var items = await _requestItemRepo.FindIncludingAsync(
                    i => i.ItemRequestId == entity.Id && !i.IsDeleted,
                    new System.Linq.Expressions.Expression<Func<RequestItem, object>>[] { i => i.ItemModel, i => i.ItemModel.ItemType });

                var itemDtos = items.Select(i => new RequestItemResponseDto
                {
                    Quantity = i.Quantity,
                    ItemModelName = i.ItemModel?.Name,
                    ItemTypeName = i.ItemModel?.ItemType?.Name
                }).ToList();

                var user = (await _userRepo.FindAsync(u => u.Id == entity.UserId && u.Active)).FirstOrDefault();
                if (user == null)
                    throw new NullObjectException(AppMessages.UserNotFound);

                result.Add(new ItemRequestResponseDto
                {
                    Id = entity.Id,
                    RequestNumber = entity.RequestNumber!,
                    Status = entity.Status!,
                    CreatedAt = entity.CreatedAt,
                    Items = itemDtos
                });
            }

            return new PagedResultDto<ItemRequestResponseDto>
            {
                Items = result,
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }


        public async Task ChangeRequestStatusAsync(int id, string status, string? comment, int userId)
        {
            var request = await _itemRequestRepo.GetByIdAsync(id);
            if (request == null)
                throw new NullObjectException(AppMessages.ItemRequestNotFound);

            var validStatuses = new[] { "Pending", "Approved", "Rejected" };
            if (!validStatuses.Contains(status))
                throw new CustomException($"Invalid status: {status}");

            // Business rules
            if (request.Status == status)
                throw new CustomException($"Request is already in '{status}' status.");

            if (request.Status != "Pending" && (status == "Approved" || status == "Rejected"))
                throw new CustomException(AppMessages.RejectRequest);

            if (status == "Approved")
            {
                var requestItems = await _requestItemRepo.FindAsync(x => x.ItemRequestId == id && !x.IsDeleted);
                foreach (var item in requestItems)
                {
                    var itemModel = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                    if (itemModel == null)
                        throw new NullObjectException($"ItemModel with ID {item.ItemModelId} not found.");
                    if (itemModel.Quantity < item.Quantity)
                        throw new CustomException($"Not enough quantity for item: {itemModel.Name}");
                }

                // Deduct quantity
                foreach (var item in requestItems)
                {
                    var itemModel = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                    if (itemModel == null)
                        throw new NullObjectException($"ItemModel with ID {item.ItemModelId} not found.");
                    //check quantity Of ItemModal with Request Quantity
                    if (itemModel.Quantity < item.Quantity)
                        throw new CustomException($"Not enough quantity for item: {itemModel.Name}");

                    itemModel.Quantity -= item.Quantity;
                    await _itemModelRepo.UpdateAsync(itemModel);
                }
            }

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;
            request.ModifiedBy = userId;
            request.Comment = comment;
            await _itemRequestRepo.UpdateAsync(request);
        }
        public async Task EditItemRequestAsync(int requestId, ItemRequestEditDto editDto, int userId)
        {
            var request = await _itemRequestRepo.GetByIdAsync(requestId);
            if (request == null)
                throw new NullObjectException(AppMessages.ItemRequestNotFound);

            if (request.UserId != userId)
                throw new CustomException(AppMessages.cannotEditOtherRequest);

            if (request.Status != "Pending")
                throw new CustomException(AppMessages.OnlyPendingReqEditable);

            var existingItems = await _requestItemRepo.FindAsync(i => i.ItemRequestId == requestId && !i.IsDeleted);
            var existingItemsDict = existingItems.ToDictionary(i => i.ItemModelId);

            foreach (var itemEdit in editDto.Items)
            {
                if (itemEdit.Quantity > 0)
                {
                    var item = await _itemModelRepo.GetByIdAsync(itemEdit.ItemModelId);
                    if (item == null || item.IsDeleted)
                        throw new CustomException(AppMessages.ItemModelNotFound);
                    if (itemEdit.Quantity > item.Quantity)
                        throw new CustomException($"Requested quantity for item {item.Name} exceeds available stock.");

                    if (existingItemsDict.TryGetValue(itemEdit.ItemModelId, out var existingItem))
                    {
                        existingItem.Quantity = itemEdit.Quantity;
                        await _requestItemRepo.UpdateAsync(existingItem);
                    }
                    else
                    {
                        var newItem = new RequestItem
                        {
                            ItemRequestId = requestId,
                            ItemModelId = itemEdit.ItemModelId,
                            Quantity = itemEdit.Quantity,
                            IsDeleted = false
                        };
                        await _requestItemRepo.AddAsync(newItem);
                    }
                }
                else if (itemEdit.Quantity == 0)
                {
                    if (existingItemsDict.TryGetValue(itemEdit.ItemModelId, out var existingItem))
                    {
                        existingItem.IsDeleted = true;
                        await _requestItemRepo.UpdateAsync(existingItem);
                    }
                }
            }

            request.UpdatedAt = DateTime.UtcNow;
            request.ModifiedBy = userId;
            await _itemRequestRepo.UpdateAsync(request);
        }
    }
}
