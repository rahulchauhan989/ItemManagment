using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels;

public class PurchaseRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required, MaxLength(50)]
    public string? InvoiceNumber { get; set; }

    public bool IsDeleted { get; set; } = false;

    [ForeignKey("User")]
    public int CreatedBy { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User? CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUser")]
    public int? ModifiedBy { get; set; }
    public User? ModifiedByUser { get; set; }
}
