
namespace ItemManagementSystem.Domain.Dto

{
    public class ItemRequestDto
    {
        public int Id { get; set; }
        public string? RequestNumber { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<RequestItemDto>? Items { get; set; }
        public string? Comment { get; set; }
    }

    public class RequestItemDto
    {
        // public int? Id { get; set; }
        // public int? RequestId { get; set; }
        public int ItemModelId { get; set; }
        public int Quantity { get; set; }

    }
}
