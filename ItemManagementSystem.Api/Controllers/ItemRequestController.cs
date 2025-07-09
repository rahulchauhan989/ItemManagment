using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/item-requests")]
    [Authorize(Roles = "Admin")]
    public class ItemRequestController : ControllerBase
    {
        private readonly IItemRequestService _itemRequestService;

        public ItemRequestController(IItemRequestService itemRequestService)
        {
            _itemRequestService = itemRequestService;
        }

        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse>> GetPendingItemRequests([FromQuery] ItemRequestFilterDto filter)
        {
            var result = await _itemRequestService.GetPendingRequestsAsync(filter);
            return new ApiResponse(true,200,result,AppMessages.ItemRequestItemsRetrieved);
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveItemRequest(int id, [FromBody] ApproveRequestDto dto)
        {
            await _itemRequestService.ApproveRequestAsync(id, dto.Comment);
            return new ApiResponse(true,200,null,AppMessages.ItemRequestApproved);
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult<ApiResponse>> RejectItemRequest(int id, [FromBody] RejectRequestDto dto)
        {
            await _itemRequestService.RejectRequestAsync(id, dto.Comment);
            return new ApiResponse(true,200,null,AppMessages.ItemRequestRejected);
        }
    }
}