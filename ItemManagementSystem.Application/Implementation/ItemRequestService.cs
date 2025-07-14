using System.Linq.Expressions;
using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataContext;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
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
        public async Task<PagedResultDto<ItemRequestDto>> GetRequestsAsync(ItemsRequestFilterDto filter)
        {
            Expression<Func<ItemRequest, bool>> predicate = r =>
            !r.IsDeleted &&
            (string.IsNullOrEmpty(filter.RequestNumber) || r.RequestNumber == filter.RequestNumber) &&
            (string.IsNullOrEmpty(filter.UserName) || (r.User != null && r.User.Name.Contains(filter.UserName)));

            Func<IQueryable<ItemRequest>, IOrderedQueryable<ItemRequest>> orderBy = filter.SortBy?.ToLower() switch
            {
                "requestnumber" => q => filter.SortDesc ? q.OrderByDescending(x => x.RequestNumber) : q.OrderBy(x => x.RequestNumber),
                "username" => q => filter.SortDesc ? q.OrderByDescending(x => x.User.Name) : q.OrderBy(x => x.User.Name),
                _ => q => filter.SortDesc ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt)
            };

            var paged = await _itemRequestRepo.GetPagedAsync(predicate, orderBy, filter.Page, filter.PageSize);

            var result = new List<ItemRequestDto>();

            foreach (var entity in paged.Items)
            {
                var items = await _requestItemRepo.FindAsync(i => i.ItemRequestId == entity.Id && !i.IsDeleted);
                var itemDtos = items.Select(i => new RequestItemDto
                {
                    ItemModelId = i.ItemModelId,
                    Quantity = i.Quantity,
                }).ToList();

                var user = (await _userRepo.FindAsync(u => u.Id == entity.UserId && u.Active)).FirstOrDefault();
                if (user == null)
                    throw new NullObjectException(AppMessages.UserNotFound);

                result.Add(new ItemRequestDto
                {
                    Id = entity.Id,

                    RequestNumber = entity.RequestNumber!,
                    Status = entity.Status!,
                    CreatedAt = entity.CreatedAt,
                    UserId = entity.UserId,
                    UserName = user.Name,
                    Items = itemDtos
                });
            }

            return new PagedResultDto<ItemRequestDto>
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
                    itemModel.Quantity -= item.Quantity;
                    await _itemModelRepo.UpdateAsync(itemModel);
                }
            }

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;
            request.ModifiedBy = userId;
            // Optionally save comment somewhere if you want (add a field to ItemRequest table if needed)
            request.Comment = comment;
            await _itemRequestRepo.UpdateAsync(request);
        }
    }
}