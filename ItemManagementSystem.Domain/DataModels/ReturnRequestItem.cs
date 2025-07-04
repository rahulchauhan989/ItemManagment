using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels;

   public class ReturnRequestItem 
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ReturnRequest")]
        public int ReturnRequestId { get; set; }
        public ReturnRequest ReturnRequest { get; set; }

        [ForeignKey("ItemModel")]
        public int ItemModelId { get; set; }
        public ItemModel ItemModel { get; set; }

        public int Quantity { get; set; }

         // Auditable fields
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CreatedByUser")]
    public int CreatedBy { get; set; }
    public User CreatedByUser { get; set; }

    [ForeignKey("ModifiedByUser")]
    public int? ModifiedBy { get; set; }
    public User? ModifiedByUser { get; set; }
    }
