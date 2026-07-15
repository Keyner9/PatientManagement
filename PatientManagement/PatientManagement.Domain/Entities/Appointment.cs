namespace PatientManagement.Domain.Entities;

public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public decimal Fee { get; set; }
    public DateTime CreatedAt { get; set; }

    public Doctor Doctor { get; set; } = null!;
}
