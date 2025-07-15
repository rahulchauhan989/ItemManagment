using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels;

public class ItemModel  
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [ForeignKey("ItemType")]
    public int ItemTypeId { get; set; }
    public ItemType ItemType { get; set; } = null!;
    public int Quantity { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedByUser")]
    public int CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUser")]
    public int? ModifiedBy { get; set; }
    public User? ModifiedByUser { get; set; }
}
