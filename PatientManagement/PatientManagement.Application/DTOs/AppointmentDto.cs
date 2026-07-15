namespace PatientManagement.Application.DTOs;

public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public decimal Fee { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
