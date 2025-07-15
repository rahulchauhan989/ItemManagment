using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
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
        private readonly IItemTypeService _itemTypeService;

        public ItemRequestController(IItemRequestService itemRequestService, IItemTypeService itemTypeService)
        {
            _itemRequestService = itemRequestService;
            _itemTypeService = itemTypeService;
        }

        [HttpGet("requests")]
        public async Task<ActionResult<ApiResponse>> GetItemRequests([FromQuery] ItemsRequestFilterDto filter)
        {
            var result = await _itemRequestService.GetRequestsAsync(filter);
            return new ApiResponse(true, 200, result, AppMessages.ItemRequestItemsRetrieved);
        }

        [HttpPost("requests")]
        public async Task<ActionResult<ApiResponse>> GetItemRequestsPost([FromBody] ItemsRequestFilterDto filter)
        {
            var result = await _itemRequestService.GetRequestsAsync(filter);
            return new ApiResponse(true, 200, result, AppMessages.ItemRequestItemsRetrieved);
        }

        [HttpPost("{id}/status")]
        public async Task<ActionResult<ApiResponse>> ChangeItemRequestStatus(int id, [FromBody] ChangeStatusDto dto)
        {
            string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _itemTypeService.ExtractUserIdFromToken(token);
            await _itemRequestService.ChangeRequestStatusAsync(id, dto.Status, dto.Comment, userId);
            return new ApiResponse(true, 200, null, $"Request status changed to {dto.Status}");
        }
    }
}