namespace ItemManagementSystem.Domain.Dto.Return;

public class ReturnsRequestDto
{
     public int ItemModelId { get; set; }
     public string? ItemModelName { get; set; } 
      public int Quantity { get; set; }

      public int? ItemTypeId { get; set; }
      public string? ItemTypeName { get; set; }
}
