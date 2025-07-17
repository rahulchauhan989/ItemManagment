using Microsoft.AspNetCore.Http;
using ItemManagementSystem.Application.Interface;

namespace ItemManagementSystem.Api.Helpers
{
    public static class UserHelper
    {
        public static int GetUserIdFromRequest(HttpRequest request, IItemTypeService itemTypeService)
        {
            string? token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return itemTypeService.ExtractUserIdFromToken(token);
        }
    }
}
