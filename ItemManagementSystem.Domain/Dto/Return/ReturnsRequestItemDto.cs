using System.ComponentModel.DataAnnotations;

namespace ItemManagementSystem.Domain.Dto.Return;

public class ReturnsRequestItemDto
{
    public int ItemModelId { get; set; }
    public int Quantity { get; set; }
    public string? ItemModelName { get; set; }
    public int ItemTypeId { get; set; }
    public string? ItemTypeName { get; set; }
}
