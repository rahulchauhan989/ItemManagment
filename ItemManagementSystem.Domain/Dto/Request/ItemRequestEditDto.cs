using System.ComponentModel.DataAnnotations;

namespace ItemManagementSystem.Domain.Dto.Request
{
    public class ItemRequestEditDto
    {
        public List<RequestItemEditDto> Items { get; set; } = new List<RequestItemEditDto>();
    }

    public class RequestItemEditDto
    {
        [Required]
        public int ItemModelId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; } 
    }
}
