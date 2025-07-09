using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<ApiResponse>> CreateRequest([FromBody] CreateItemRequestDto dto)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var response = await _service.CreateRequestAsync(userId, dto);
        return new ApiResponse(true,201,response,AppMessages.UserCreatedItemReq);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyRequests()
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var list = await _service.GetRequestsByUserAsync(userId);
        return new ApiResponse(true, 200, list, AppMessages.GetMyRequests);
    }

    [HttpPatch("{requestId}/status")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ChangeStatus(int requestId, [FromQuery] string status)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        bool updated = await _service.ChangeStatusAsync(requestId, status, userId);
        if (!updated) return BadRequest("Invalid status or request not found.");
        return new ApiResponse(true, 200, null, "Request status updated successfully.");
    }
}