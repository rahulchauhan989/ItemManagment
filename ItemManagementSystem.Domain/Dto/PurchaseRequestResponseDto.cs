using System;
using System.Collections.Generic;

namespace ItemManagementSystem.Domain.Dto
{
    public class PurchaseRequestResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CreatedBy { get; set; }
        public List<PurchaseRequestItemResponseDto> Items { get; set; } = new List<PurchaseRequestItemResponseDto>();
    }

    public class PurchaseRequestItemResponseDto
    {
        public string? Name { get; set; }
        public string? ItemType { get; set; }
        public int Quantity { get; set; }
    }
}
