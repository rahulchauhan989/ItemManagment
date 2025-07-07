using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels;

public class ItemRequest
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string? RequestNumber { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [ForeignKey("PurchaseRequest")]
    public int PurchaseId { get; set; }
    public PurchaseRequest? PurchaseRequest { get; set; }

    [Required, MaxLength(50)]
    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedByUser")]
    public int CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUser")]
    public int? ModifiedBy { get; set; }
    public User? ModifiedByUser { get; set; }
}
