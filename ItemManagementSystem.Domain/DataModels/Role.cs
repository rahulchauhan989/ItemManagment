using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels;

public class Role 
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;


    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedByUser")]
    public int? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUser")]
    public int? ModifiedBy { get; set; }
    public User? ModifiedByUser { get; set; }
}

