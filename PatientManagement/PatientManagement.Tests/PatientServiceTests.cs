using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagement.Application.Common;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Services;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Tests;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        _repositoryMock = new Mock<IPatientRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<PatientService>>();
        _service = new PatientService(
            _repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    private static Patient CreatePatient(int id = 1) => new()
    {
        PatientId = id,
        DocumentType = "DNI",
        DocumentNumber = "12345678",
        FirstName = "Carlos",
        LastName = "Garcia",
        BirthDate = new DateOnly(1985, 3, 15),
        PhoneNumber = "555-0101",
        Email = "carlos@email.com",
        CreatedAt = new DateTime(2026, 7, 2, 0, 0, 0, DateTimeKind.Utc)
    };

    private static CreatePatientDto CreateCreateDto() => new()
    {
        DocumentType = "DNI",
        DocumentNumber = "12345678",
        FirstName = "Carlos",
        LastName = "Garcia",
        BirthDate = new DateOnly(1985, 3, 15),
        PhoneNumber = "555-0101",
        Email = "carlos@email.com"
    };

    private static UpdatePatientDto CreateUpdateDto() => new()
    {
        DocumentType = "DNI",
        DocumentNumber = "12345678",
        FirstName = "Carlos Updated",
        LastName = "Garcia Updated",
        BirthDate = new DateOnly(1985, 3, 15),
        PhoneNumber = "555-9999",
        Email = "carlos.updated@email.com"
    };

    private static PatientDto CreatePatientDto(int id = 1) => new()
    {
        PatientId = id,
        DocumentType = "DNI",
        DocumentNumber = "12345678",
        FirstName = "Carlos",
        LastName = "Garcia",
        BirthDate = new DateOnly(1985, 3, 15),
        PhoneNumber = "555-0101",
        Email = "carlos@email.com",
        CreatedAt = new DateTime(2026, 7, 2, 0, 0, 0, DateTimeKind.Utc)
    };

    [Fact]
    public async Task CreateAsync_ShouldCreatePatient_WhenDataIsValid()
    {
        var dto = CreateCreateDto();
        var patient = CreatePatient();
        var patientDto = CreatePatientDto();

        _repositoryMock
            .Setup(r => r.ExistsAsync(dto.DocumentType, dto.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(m => m.Map<Patient>(It.IsAny<object>()))
            .Returns(patient);

        _mapperMock
            .Setup(m => m.Map<PatientDto>(It.IsAny<object>()))
            .Returns(patientDto);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Patient created successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.PatientId);
        Assert.Equal("Carlos", result.Data.FirstName);
        _repositoryMock.Verify(r => r.ExistsAsync(dto.DocumentType, dto.DocumentNumber, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowDuplicatePatientException_WhenDuplicateExists()
    {
        var dto = CreateCreateDto();

        _repositoryMock
            .Setup(r => r.ExistsAsync(dto.DocumentType, dto.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DuplicatePatientException>(
            () => _service.CreateAsync(dto, CancellationToken.None));

        Assert.Contains("DNI", exception.Message);
        Assert.Contains("12345678", exception.Message);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToUtcNow_WhenDataIsValid()
    {
        var dto = CreateCreateDto();
        var patient = CreatePatient();

        _repositoryMock
            .Setup(r => r.ExistsAsync(dto.DocumentType, dto.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(m => m.Map<Patient>(It.IsAny<object>()))
            .Returns(patient);

        _mapperMock
            .Setup(m => m.Map<PatientDto>(It.IsAny<object>()))
            .Returns(CreatePatientDto());

        Patient? capturedPatient = null;
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync(patient);

        await _service.CreateAsync(dto, CancellationToken.None);

        Assert.NotNull(capturedPatient);
        Assert.Equal(DateTimeKind.Utc, capturedPatient!.CreatedAt.Kind);
        Assert.True((DateTime.UtcNow - capturedPatient.CreatedAt).TotalSeconds < 5,
            "CreatedAt should be set to a recent UtcNow value");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPatient_WhenExists()
    {
        var patient = CreatePatient();
        var patientDto = CreatePatientDto();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        _mapperMock
            .Setup(m => m.Map<PatientDto>(It.IsAny<object>()))
            .Returns(patientDto);

        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.PatientId);
        Assert.Equal("Carlos", result.Data.FirstName);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(999, CancellationToken.None));

        Assert.Contains("999", exception.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResult_WhenDataExists()
    {
        var filter = new PatientFilterDto { Page = 1, PageSize = 10 };
        var patients = new List<Patient> { CreatePatient(1), CreatePatient(2) };
        var dtos = new List<PatientListDto>
        {
            new() { PatientId = 1, FirstName = "Carlos" },
            new() { PatientId = 2, FirstName = "Maria" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<string?>(), It.IsAny<string?>(),
                filter.Page, filter.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((patients, 2));

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PatientListDto>>(It.IsAny<object>()))
            .Returns(dtos);

        var result = await _service.GetAllAsync(filter, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalCount);
        Assert.Equal(1, result.Data.Page);
        Assert.Equal(10, result.Data.PageSize);
        Assert.Equal(1, result.Data.TotalPages);
        Assert.False(result.Data.HasPreviousPage);
        Assert.False(result.Data.HasNextPage);
        Assert.Equal(2, result.Data.Items.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePatient_WhenDataIsValid()
    {
        var id = 1;
        var existingPatient = CreatePatient(id);
        var updateDto = CreateUpdateDto();
        var patientDto = CreatePatientDto(id);
        patientDto.FirstName = "Carlos Updated";
        patientDto.LastName = "Garcia Updated";

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPatient);

        _mapperMock
            .Setup(m => m.Map(It.IsAny<UpdatePatientDto>(), It.IsAny<Patient>()))
            .Returns<UpdatePatientDto, Patient>((src, dest) =>
            {
                dest.FirstName = src.FirstName;
                dest.LastName = src.LastName;
                dest.DocumentType = src.DocumentType;
                dest.DocumentNumber = src.DocumentNumber;
                dest.BirthDate = src.BirthDate;
                dest.PhoneNumber = src.PhoneNumber;
                dest.Email = src.Email;
                return dest;
            });

        _mapperMock
            .Setup(m => m.Map<PatientDto>(It.IsAny<object>()))
            .Returns(patientDto);

        var result = await _service.UpdateAsync(id, updateDto, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Patient updated successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("Carlos Updated", result.Data.FirstName);
        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenPatientNotExists()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAsync(999, CreateUpdateDto(), CancellationToken.None));

        Assert.Contains("999", exception.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowDuplicatePatientException_WhenDocumentChangedAndAlreadyExists()
    {
        var id = 1;
        var existingPatient = CreatePatient(id);
        var updateDto = CreateUpdateDto();
        updateDto.DocumentType = "CE";
        updateDto.DocumentNumber = "87654321";

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPatient);

        _repositoryMock
            .Setup(r => r.ExistsAsync(updateDto.DocumentType, updateDto.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DuplicatePatientException>(
            () => _service.UpdateAsync(id, updateDto, CancellationToken.None));

        Assert.Contains("CE", exception.Message);
        Assert.Contains("87654321", exception.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePatient_WhenExists()
    {
        var id = 1;
        var patient = CreatePatient(id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);

        var result = await _service.DeleteAsync(id, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Patient deleted successfully.", result.Message);
        Assert.Null(result.Data);
        _repositoryMock.Verify(r => r.DeleteAsync(patient, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenPatientNotExists()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAsync(999, CancellationToken.None));

        Assert.Contains("999", exception.Message);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
