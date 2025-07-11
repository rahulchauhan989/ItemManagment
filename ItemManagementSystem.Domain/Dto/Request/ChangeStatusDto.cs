namespace ItemManagementSystem.Domain.Dto.Request;

public class ChangeStatusDto
{
    public string Status { get; set; } = null!;
    public string? Comment { get; set; }
}