using ItemManagementSystem.Application.Interface;
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
    public async Task<IActionResult> CreateReturnRequest([FromBody] ReturnRequestCreateDto dto)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var result = await _returnRequestService.CreateReturnRequestAsync(userId, dto);
        return Ok(result);
    }



    [HttpPost("my-requests")]
    public async Task<IActionResult> GetUserReturnRequestsPost([FromBody] ReturnRequestFilterDto filter)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token);
        var result = await _returnRequestService.GetUserReturnRequestsAsync(userId, filter);
        return Ok(result);
    }

 

    [HttpPost("all-request")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReturnRequestsPost([FromBody] ReturnRequestFilterDto filter)
    {
        var result = await _returnRequestService.GetAllReturnRequestsAsync(filter);
        return Ok(result);
    }

    [HttpPut("status-update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status, [FromQuery] string? comment)
    {
        string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        int userId = _itemTypeService.ExtractUserIdFromToken(token); 
        await _returnRequestService.UpdateReturnRequestStatusAsync(id, status, comment, userId);
        return Ok();
    }
}
