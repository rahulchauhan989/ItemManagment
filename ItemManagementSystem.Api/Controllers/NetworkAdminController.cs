using ItemManagementSystem.Application.Interface;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.Dto;
using ItemManagementSystem.Domain.Dto.Request;
using ItemManagementSystem.Domain.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/network-admin")]
    [Authorize(Roles = "Admin")]
    public class NetworkAdminController : ControllerBase
    {
        private readonly IItemTypeService _itemTypeService;
        private readonly IItemModelService _itemModelService;
        private readonly IPurchaseRequestService _purchaseRequestService;

        public NetworkAdminController(
            IItemTypeService itemTypeService,
            IItemModelService itemModelService,
            IPurchaseRequestService purchaseRequestService
            )
        {
            _itemTypeService = itemTypeService;
            _itemModelService = itemModelService;
            _purchaseRequestService = purchaseRequestService;
        }


        [HttpPost("item-types")]
        public async Task<ActionResult<ApiResponse>> CreateItemType([FromBody] ItemTypeCreateRequest request)
        {
            string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _itemTypeService.ExtractUserIdFromToken(token);

            var response = await _itemTypeService.CreateAsync(request, userId);

            return new ApiResponse(true, 201, response, AppMessages.ItemTypeCreated);
        }

        [HttpGet("item-types/{id}")]
        public async Task<ActionResult<ApiResponse>> GetItemType(int id)
        {
            var result = await _itemTypeService.GetByIdAsync(id);
            return new ApiResponse(true, 200, result, AppMessages.ItemTypesRetrieved);
        }

          [HttpPost("item-types/search")]
        public async Task<ActionResult<ApiResponse>> FilterItemTypes([FromBody] ItemTypeFilterDto filter)
        {
            var result = await _itemTypeService.GetPagedItemTypesAsync(filter);
            return new ApiResponse(true, 200, result, AppMessages.ItemTypesRetrieved);
        }

        [HttpPut("item-types/{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateItemType(int id, [FromBody] ItemTypeCreateRequest dto)
        {
            string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _itemTypeService.ExtractUserIdFromToken(token);

            var result = await _itemTypeService.updateAsync(id, dto, userId);
            return new ApiResponse(true, 204, result, AppMessages.ItemTypeUpdated);
        }

        [HttpDelete("item-types/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteItemType(int id)
        {
            await _itemTypeService.DeleteAsync(id);
            return new ApiResponse(true, 204, null, AppMessages.ItemTypeDeleted);
        }

        // [HttpGet("item-types")]
        // public async Task<ActionResult<ApiResponse>> GetItemTypes([FromQuery] ItemTypeFilterDto filter)
        // {
        //     var result = await _itemTypeService.GetPagedItemTypesAsync(filter);
        //     return new ApiResponse(true, 200, result, AppMessages.ItemTypesRetrieved);
        // }

        [HttpPost("item-models")]
        public async Task<ActionResult<ApiResponse>> CreateItemModel([FromBody] ItemModelCreateDto dto)
        {
            string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _itemTypeService.ExtractUserIdFromToken(token);
            var result = await _itemModelService.CreateAsync(dto, userId);
            return new ApiResponse(true, 201, result, AppMessages.ItemModelCreated);
        }

        [HttpGet("item-models/{id}")]
        public async Task<ActionResult<ApiResponse>> GetItemModel(int id)
        {
            var result = await _itemModelService.GetByIdAsync(id);
            return new ApiResponse(true, 200, result, AppMessages.ItemModelsRetrieved);
        }

      

        [HttpPut("item-models/{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateItemModel(int id, [FromBody] ItemModelCreateDto dto)
        {
            string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _itemTypeService.ExtractUserIdFromToken(token);
            var result = await _itemModelService.UpdateAsync(id, dto, userId);
            return new ApiResponse(true, 204, result, AppMessages.ItemModelUpdated);
        }

        [HttpDelete("item-models/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteItemModel(int id)
        {
            await _itemModelService.DeleteAsync(id);
            return new ApiResponse(true, 204, null, AppMessages.ItemModelDeleted);
        }

        // [HttpGet("item-models")]
        // public async Task<ActionResult<ApiResponse>> GetAllItemModels([FromQuery] ItemModelFilterDto filter)
        // {
        //     var result = await _itemModelService.GetPagedAsync(filter);
        //     return new ApiResponse(true, 200, result, AppMessages.ItemModelsRetrieved);
        // }

        [HttpPost("item-models/search")]
        public async Task<ActionResult<ApiResponse>> FilterItemModels([FromBody] ItemModelFilterDto filter)
        {
            var result = await _itemModelService.GetPagedAsync(filter);
            return new ApiResponse(true, 200, result, AppMessages.ItemModelsRetrieved);
        }

        [HttpPost("purchase-requests")]
        public async Task<ActionResult<ApiResponse>> CreatePurchaseRequest([FromBody] PurchaseRequestCreateDto dto)
        {
            string? token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _itemTypeService.ExtractUserIdFromToken(token);

            var result = await _purchaseRequestService.CreateAsync(dto, userId);
            return new ApiResponse(true, 201, result, AppMessages.PurchaseRequestCreated);
        }

        [HttpGet("purchase-requests/{id}")]
        public async Task<ActionResult<ApiResponse>> GetPurchaseRequest(int id)
        {
            var result = await _purchaseRequestService.GetByIdAsync(id);
            return new ApiResponse(true, 200, result, AppMessages.PurchaseRequestsRetrieved);
        }

        // [HttpGet("purchase-requests")]
        // public async Task<ActionResult<ApiResponse>> ListPurchaseRequests([FromQuery] PurchaseRequestFilterDto filter)
        // {
        //     var result = await _purchaseRequestService.GetAllAsync(filter);
        //     return new ApiResponse(true, 200, result, AppMessages.PurchaseRequestsRetrieved);
        // }

        [HttpPost("purchase-requests/search")]
        public async Task<ActionResult<ApiResponse>> FilterPurchaseRequests([FromBody] PurchaseRequestFilterDto filter)
        {
            var result = await _purchaseRequestService.GetAllAsync(filter);
            return new ApiResponse(true, 200, result, AppMessages.PurchaseRequestsRetrieved);
        }


    }
}