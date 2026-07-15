using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Patient> Items, int TotalCount)> GetAllAsync(
        string? name, string? documentNumber, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default);
    Task<Patient> CreateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetPatientsCreatedAfterAsync(DateTime createdAfter, CancellationToken cancellationToken = default);
}
