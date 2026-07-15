using AutoMapper;
using Microsoft.Extensions.Logging;
using PatientManagement.Application.Common;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository repository,
        IMapper mapper,
        ILogger<PatientService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PatientDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException($"No se encontro el paciente con id {id}.");
        }

        var dto = _mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.Ok(dto);
    }

    public async Task<ApiResponse<PagedResult<PatientListDto>>> GetAllAsync(
        PatientFilterDto filter, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _repository.GetAllAsync(
            filter.Name, filter.DocumentNumber, filter.Page, filter.PageSize, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<PatientListDto>>(items);

        var result = new PagedResult<PatientListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        return ApiResponse<PagedResult<PatientListDto>>.Ok(result);
    }

    public async Task<ApiResponse<PatientDto>> CreateAsync(
        CreatePatientDto dto, CancellationToken cancellationToken = default)
    {
        var duplicate = await _repository.ExistsAsync(dto.DocumentType, dto.DocumentNumber, cancellationToken);

        if (duplicate)
        {
            throw new DuplicatePatientException(
                $"Ya existe un paciente con el documento {dto.DocumentType} {dto.DocumentNumber}.");
        }

        var patient = _mapper.Map<Patient>(dto);
        patient.CreatedAt = DateTime.UtcNow;

        var created = await _repository.CreateAsync(patient, cancellationToken);

        _logger.LogInformation("Patient {DocumentType} {DocumentNumber} created with id {PatientId}",
            created.DocumentType, created.DocumentNumber, created.PatientId);

        var resultDto = _mapper.Map<PatientDto>(created);
        return ApiResponse<PatientDto>.Ok(resultDto, "Paciente creado correctamente.");
    }

    public async Task<ApiResponse<PatientDto>> UpdateAsync(
        int id, UpdatePatientDto dto, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException($"No se encontro el paciente con id {id}.");
        }

        var documentChanged = patient.DocumentType != dto.DocumentType || patient.DocumentNumber != dto.DocumentNumber;

        if (documentChanged)
        {
            var duplicate = await _repository.ExistsAsync(dto.DocumentType, dto.DocumentNumber, cancellationToken);

            if (duplicate)
            {
                throw new DuplicatePatientException(
                    $"Ya existe un paciente con el documento {dto.DocumentType} {dto.DocumentNumber}.");
            }
        }

        _mapper.Map(dto, patient);

        await _repository.UpdateAsync(patient, cancellationToken);

        _logger.LogInformation("Patient {PatientId} updated", id);

        var resultDto = _mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.Ok(resultDto, "Paciente actualizado correctamente.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException($"No se encontro el paciente con id {id}.");
        }

        await _repository.DeleteAsync(patient, cancellationToken);

        _logger.LogInformation("Patient {PatientId} deleted", id);

        return ApiResponse<object>.Ok(null!, "Paciente eliminado correctamente.");
    }

    public async Task<ApiResponse<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(patientId, cancellationToken);

        if (patient is null)
        {
            throw new NotFoundException($"No se encontro el paciente con id {patientId}.");
        }

        var appointments = await _repository.GetAppointmentsByPatientIdAsync(patientId, cancellationToken);

        var dtos = appointments.Select(a => new AppointmentDto
        {
            AppointmentId = a.AppointmentId,
            AppointmentDate = a.AppointmentDate,
            Diagnosis = a.Diagnosis,
            Treatment = a.Treatment,
            Fee = a.Fee,
            DoctorName = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
            DoctorSpecialty = a.Doctor.Specialty,
            CreatedAt = a.CreatedAt,
        });

        return ApiResponse<IEnumerable<AppointmentDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<IEnumerable<PatientListDto>>> GetPatientsCreatedAfterAsync(DateTime createdAfter, CancellationToken cancellationToken = default)
    {
        var patients = await _repository.GetPatientsCreatedAfterAsync(createdAfter, cancellationToken);
        var dtos = _mapper.Map<IEnumerable<PatientListDto>>(patients);
        return ApiResponse<IEnumerable<PatientListDto>>.Ok(dtos);
    }
}
