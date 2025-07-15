using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers;

[ApiController]
[Route("api/user-item-requests")]
public class UserItemRequestController : ControllerBase
{
    private readonly IUserItemRequestService _service;
    private readonly IItemTypeService _itemTypeService;

    public UserItemRequestController(IUserItemRequestService service, IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
        _service = service;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateUserItemRequest([FromBody] CreateItemRequestDto dto)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var response = await _service.CreateRequestAsync(userId, dto);
        return new ApiResponse(true, 201, response, AppMessages.UserCreatedItemReq);
    }


    [HttpGet("mine")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyItemRequests([FromQuery] Domain.Dto.Request.ItemRequestFilterDto filter)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var pagedList = await _service.GetRequestsByUserPagedAsync(userId, filter);
        return new ApiResponse(true, 200, pagedList, AppMessages.GetMyRequests);
    }

    [HttpPost("mine")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyItemRequestsPost([FromBody] Domain.Dto.Request.ItemRequestFilterDto filter)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var pagedList = await _service.GetRequestsByUserPagedAsync(userId, filter);
        return new ApiResponse(true, 200, pagedList, AppMessages.GetMyRequests);
    }
    
    [HttpPatch("{requestId}/cancel")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CancelItemRequest(int requestId)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        await _service.ChangeStatusAsync(requestId, userId);
        return new ApiResponse(true, 200, null, AppMessages.RequestCancelled);
    }
}
