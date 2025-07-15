using System.ComponentModel.DataAnnotations;

namespace ItemManagementSystem.Domain.Dto.Request;

public class PurchaseRequestCreateDto
{
    // public DateTime Date { get; set; }
    public List<PurchaseRequestItemCreateDto> Items { get; set; } = null!;
}

public class PurchaseRequestItemCreateDto
{
    public int ItemModelId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
}