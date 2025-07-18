using ItemManagementSystem.Api.Helpers;
using ItemManagementSystem.Application.Implementation;
using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers;

[ApiController]
[Route("api/user-item-requests")]
public class UserItemRequestController : ControllerBase
{
    private readonly IUserItemRequestService _UserItemReqService;
    private readonly IItemTypeService _itemTypeService;

    private readonly IItemRequestService _itemRequestService;

    public UserItemRequestController(IUserItemRequestService service, IItemTypeService itemTypeService, IItemRequestService itemRequestService)
    {
        _itemTypeService = itemTypeService;
        _UserItemReqService = service;
        _itemRequestService = itemRequestService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CreateUserItemRequest([FromBody] CreateItemRequestDto dto)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        var response = await _UserItemReqService.CreateRequestAsync(userId, dto);
        return new ApiResponse(true, 201, null, AppMessages.UserCreatedItemReq);
    }

    [HttpPost("mine")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMyItemRequestsPost([FromBody] Domain.Dto.Request.ItemRequestFilterDto filter)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        var pagedList = await _UserItemReqService.GetRequestsByUserPagedAsync(userId, filter);
        return new ApiResponse(true, 200, pagedList, AppMessages.GetMyRequests);
    }
    
    [HttpPatch("{requestId}/cancel")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CancelItemRequest(int requestId)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _UserItemReqService.ChangeStatusAsync(requestId, userId);
        return new ApiResponse(true, 200, null, AppMessages.RequestCancelled);
    }

    [HttpPost("save-draft")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> SaveDraft([FromBody] CreateItemRequestDto dto)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _UserItemReqService.SaveDraftAsync(userId, dto);
        return new ApiResponse(true, 200, null, AppMessages.ItemSavedAsDraft);
    }

    [HttpPut("draft-to-pending/{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DraftToPending(int id)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _UserItemReqService.ChangeDraftToPendingAsync(id, userId);
        return new ApiResponse(true, 200, null, AppMessages.ItemRequestStatusDraftToPending);
    }

    [HttpPut("{id}/edit")]
    public async Task<ActionResult<ApiResponse>> EditItemRequest(int id, [FromBody] ItemRequestEditDto editDto)
    {
        int userId =  UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        await _itemRequestService.EditItemRequestAsync(id, editDto, userId);
        return new ApiResponse(true, 200, null, AppMessages.ItemRequestUpdated);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetUserItemRequestById(int id)
    {
        int userId = UserHelper.GetUserIdFromRequest(Request, _itemTypeService);
        var result = await _UserItemReqService.GetUserItemRequestByIdAsync(id);
        if (result == null)
        {
            return NotFound(new ApiResponse(false, 404, null, AppMessages.ItemRequestNotFound));
        }
        return new ApiResponse(true, 200, result, AppMessages.GetRequestDetails);
    }

}
