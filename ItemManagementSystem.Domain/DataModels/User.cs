using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public required Role Role { get; set; }

    public bool Active { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedByUser")]
    public int? CreatedBy { get; set; }
    public User? CreatedByUser { get; set; }
    public bool IsDeleted { get; set; } = false;


    [ForeignKey("ModifiedByUser")]
    public int? ModifiedBy { get; set; }
    public User? ModifiedByUser { get; set; }
}
