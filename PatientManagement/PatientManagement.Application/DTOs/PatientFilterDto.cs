namespace PatientManagement.Application.DTOs;

public class PatientFilterDto
{
    public string? Name { get; set; }
    public string? DocumentNumber { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
