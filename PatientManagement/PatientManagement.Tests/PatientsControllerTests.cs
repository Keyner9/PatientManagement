using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagement.API.Controllers;
using PatientManagement.Application.Common;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Interfaces;

namespace PatientManagement.Tests;

public class PatientsControllerTests
{
    private readonly Mock<IPatientService> _serviceMock;
    private readonly Mock<ILogger<PatientsController>> _loggerMock;
    private readonly PatientsController _controller;

    public PatientsControllerTests()
    {
        _serviceMock = new Mock<IPatientService>();
        _loggerMock = new Mock<ILogger<PatientsController>>();
        _controller = new PatientsController(_serviceMock.Object, _loggerMock.Object);
    }

    private static ApiResponse<PatientDto> CreatePatientResponse(int id = 1) =>
        ApiResponse<PatientDto>.Ok(new PatientDto
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
        });

    [Fact]
    public async Task GetAllAsync_ShouldReturnOkObjectResult_WhenCalled()
    {
        var filter = new PatientFilterDto();
        var pagedResult = new PagedResult<PatientListDto>
        {
            Items = new List<PatientListDto>(),
            TotalCount = 0,
            Page = 1,
            PageSize = 10
        };
        var serviceResponse = ApiResponse<PagedResult<PatientListDto>>.Ok(pagedResult);

        _serviceMock
            .Setup(s => s.GetAllAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var result = await _controller.GetAllAsync(filter, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedResult<PatientListDto>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(0, response.Data.TotalCount);
        Assert.Equal(1, response.Data.Page);
        Assert.Equal(10, response.Data.PageSize);
        _serviceMock.Verify(s => s.GetAllAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOkObjectResult_WhenPatientExists()
    {
        var serviceResponse = CreatePatientResponse(5);

        _serviceMock
            .Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var result = await _controller.GetByIdAsync(5, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(5, response.Data.PatientId);
        Assert.Equal("Carlos", response.Data.FirstName);
        _serviceMock.Verify(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedAtRouteResult_WhenPatientCreated()
    {
        var dto = new CreatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };
        var serviceResponse = CreatePatientResponse(1);
        serviceResponse.Message = "Patient created successfully.";

        _serviceMock
            .Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var result = await _controller.CreateAsync(dto, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal("GetPatientById", createdResult.RouteName);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(1, createdResult.RouteValues["id"]);

        var response = Assert.IsType<ApiResponse<PatientDto>>(createdResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Patient created successfully.", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.PatientId);
        _serviceMock.Verify(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnOkObjectResult_WhenPatientUpdated()
    {
        var id = 3;
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos Updated",
            LastName = "Garcia Updated",
            BirthDate = new DateOnly(1985, 3, 15)
        };
        var serviceResponse = CreatePatientResponse(id);
        serviceResponse.Data!.FirstName = "Carlos Updated";
        serviceResponse.Data!.LastName = "Garcia Updated";

        _serviceMock
            .Setup(s => s.UpdateAsync(id, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var result = await _controller.UpdateAsync(id, dto, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PatientDto>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.PatientId);
        Assert.Equal("Carlos Updated", response.Data.FirstName);
        Assert.Equal("Garcia Updated", response.Data.LastName);
        _serviceMock.Verify(s => s.UpdateAsync(id, dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContentResult_WhenPatientDeleted()
    {
        var id = 2;
        var serviceResponse = ApiResponse<object>.Ok(null!, "Patient deleted successfully.");

        _serviceMock
            .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var result = await _controller.DeleteAsync(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
