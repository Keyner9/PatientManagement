using PatientManagement.Application.Common;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Interfaces;

public interface IPatientService
{
    Task<ApiResponse<PatientDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResult<PatientListDto>>> GetAllAsync(PatientFilterDto filter, CancellationToken cancellationToken = default);
    Task<ApiResponse<PatientDto>> CreateAsync(CreatePatientDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<PatientDto>> UpdateAsync(int id, UpdatePatientDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(int patientId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<PatientListDto>>> GetPatientsCreatedAfterAsync(DateTime createdAfter, CancellationToken cancellationToken = default);
}
