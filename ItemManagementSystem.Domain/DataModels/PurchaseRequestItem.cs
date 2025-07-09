using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemManagementSystem.Domain.DataModels
{
    public class PurchaseRequestItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PurchaseRequest")]
        public int PurchaseRequestId { get; set; }
        public PurchaseRequest PurchaseRequest { get; set; } = null!;

        [ForeignKey("ItemModel")]
        public int ItemModelId { get; set; }
        public ItemModel ItemModel { get; set; } = null!;

        public int Quantity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        public User? CreatedByUser { get; set; }
    }
}