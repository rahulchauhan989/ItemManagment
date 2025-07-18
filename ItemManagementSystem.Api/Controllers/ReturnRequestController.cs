using ItemManagementSystem.Api.Helpers;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers;

[ApiController]
[Route("api/return-request")]
[Authorize]
public class ReturnRequestController : ControllerBase
{
    private readonly IReturnRequestService _returnRequestService;
    private readonly IItemTypeService _itemTypeService;

    public ReturnRequestController(IReturnRequestService returnRequestService, IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
        _returnRequestService = returnRequestService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse>> CreateReturnRequest([FromBody] ReturnRequestCreateDto dto)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        var result = await _returnRequestService.CreateReturnRequestAsync(userId, dto);
        return new ApiResponse(true, 201, null, AppMessages.ReturnRequestCreated);
    }

    [HttpPost("my-requests")]
    public async Task<ActionResult<ApiResponse>> GetUserReturnRequestsPost([FromBody] ReturnRequestFilterDto filter)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        var result = await _returnRequestService.GetUserReturnRequestsAsync(userId, filter);
        return new ApiResponse(true, 200, result, AppMessages.GetMyReturnRequests);
    }

    [HttpPost("all-request")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> GetAllReturnRequestsPost([FromBody] ReturnRequestFilterDto filter)
    {
        var result = await _returnRequestService.GetAllReturnRequestsAsync(filter);
        return new ApiResponse(true, 200, result, AppMessages.GetAllReturnRequests);
    }

    [HttpPut("status-update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse>> UpdateStatus(int id, [FromQuery] string status, [FromQuery] string? comment)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _returnRequestService.UpdateReturnRequestStatusAsync(id, status, comment, userId);
        return new ApiResponse(true, 200, null, $"Return request status updated to {status}");
    }

    [HttpPut("edit/{id}")]
    public async Task<ActionResult<ApiResponse>> EditReturnRequest(int id, [FromBody] ReturnRequestCreateDto dto)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _returnRequestService.EditReturnRequestAsync(id, userId, dto);
        return new ApiResponse(true, 204, null, AppMessages.ReturnRequestUpdated);
    }

    [HttpPost("cancel/{id}")]
    public async Task<ActionResult<ApiResponse>> CancelReturnRequest(int id)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _returnRequestService.CancelReturnRequestAsync(id, userId);
        return new ApiResponse(true, 200, null, AppMessages.ReturnRequestCancelled);
    }

    [HttpPost("save-draft")]
    public async Task<ActionResult<ApiResponse>> SaveDraft([FromBody] ReturnRequestCreateDto dto)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _returnRequestService.SaveDraftAsync(userId, dto);
        return new ApiResponse(true, 200, null, AppMessages.ReturnRequestDraftSaved);
    }

    [HttpPut("draft-to-pending/{id}")]
    public async Task<ActionResult<ApiResponse>> DraftToPending(int id)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _returnRequestService.ChangeDraftToPendingAsync(id, userId);
        return new ApiResponse(true, 200, null, AppMessages.ReturnRequestDraftToPending);
    }
}
