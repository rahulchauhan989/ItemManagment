using AutoMapper;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Exception;
using ItemManagementSystem.Infrastructure.Interface;


namespace ItemManagementSystem.Application.Implementation
{
    public class ItemRequestService : IItemRequestService
    {
        private readonly IRepository<ItemRequest> _itemRequestRepo;
        private readonly IRepository<RequestItem> _requestItemRepo;
        private readonly IRepository<ItemModel> _itemModelRepo;
        private readonly IMapper _mapper;

        public ItemRequestService(
            IRepository<ItemRequest> itemRequestRepo,
            IRepository<RequestItem> requestItemRepo,
            IRepository<ItemModel> itemModelRepo,
            IMapper mapper)
        {
            _itemRequestRepo = itemRequestRepo;
            _requestItemRepo = requestItemRepo;
            _itemModelRepo = itemModelRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ItemRequestDto>> GetPendingRequestsAsync(ItemRequestFilterDto filter)
        {
            var allRequests = await _itemRequestRepo.FindAsync(r => r.Status == "Pending" && !r.IsDeleted);

            if (!string.IsNullOrEmpty(filter.RequestNumber))
                allRequests = allRequests.Where(r => r.RequestNumber == filter.RequestNumber);

            if (!string.IsNullOrEmpty(filter.UserName))
                allRequests = allRequests.Where(r => r.User != null && r.User.Name.Contains(filter.UserName));

            int skip = (filter.Page - 1) * filter.PageSize;
            var paged = allRequests.Skip(skip).Take(filter.PageSize).ToList();

            // Populate RequestItems for each ItemRequest
            foreach (var req in paged)
            {
                req.RequestItems = (await _requestItemRepo.FindAsync(ri => ri.RequestId == req.Id)).ToList();
            }

            return _mapper.Map<IEnumerable<ItemRequestDto>>(paged);
        }

        public async Task ApproveRequestAsync(int id, string comment)
        {
            var request = await _itemRequestRepo.GetByIdAsync(id);
            if (request == null)
                throw new NullObjectException(AppMessages.ItemRequestNotFound);

            if (request.Status != "Pending")
                throw new CustomException("Request is not in Pending status.");

            var requestItems = await _requestItemRepo.FindAsync(x => x.RequestId == id && !x.IsDeleted);

            // Check if enough quantity in ItemModels for every requested item
            foreach (var item in requestItems)
            {
                var itemModel = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                if (itemModel == null)
                    throw new NullObjectException($"ItemModel with ID {item.ItemModelId} not found.");

                if (itemModel.Quantity < item.Quantity)
                    throw new CustomException($"Not enough quantity for item: {itemModel.Name}");
            }

            // Deduct requested quantity from ItemModels
            foreach (var item in requestItems)
            {
                var itemModel = await _itemModelRepo.GetByIdAsync(item.ItemModelId);
                if (itemModel == null)
                    throw new NullObjectException($"ItemModel with ID {item.ItemModelId} not found.");
                itemModel.Quantity -= item.Quantity;
                await _itemModelRepo.UpdateAsync(itemModel);
            }

            request.Status = "Approved";
            request.UpdatedAt = DateTime.UtcNow;
            // Optionally store approval comment somewhere if needed
            await _itemRequestRepo.UpdateAsync(request);
        }

        public async Task RejectRequestAsync(int id, string comment)
        {
            var request = await _itemRequestRepo.GetByIdAsync(id);
            if (request == null)
                throw new NullObjectException(AppMessages.ItemRequestNotFound);

            if (request.Status != "Pending")
                throw new CustomException("Request is not in Pending status.");

            request.Status = "Rejected";
            request.UpdatedAt = DateTime.UtcNow;
            // Optionally store rejection comment somewhere if needed
            await _itemRequestRepo.UpdateAsync(request);
        }
    }
}