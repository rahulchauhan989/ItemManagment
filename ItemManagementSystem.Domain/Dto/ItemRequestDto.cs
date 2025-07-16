
namespace ItemManagementSystem.Domain.Dto

{
    public class ItemRequestDto
    {
        public int Id { get; set; }
        public string? RequestNumber { get; set; }
        public string? UserName { get; set; }

        public string? ItemTypeName { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<RequestItemDto>? Items { get; set; }
        public string? Comment { get; set; }
    }

    public class RequestItemDto
    {
        public int ItemModelId { get; set; }
        public int Quantity { get; set; }
        public int? ItemTypeId { get; set; }
        public string? ItemTypeName { get; set; }
        public string? ItemModelName { get; set; }


    }
}
