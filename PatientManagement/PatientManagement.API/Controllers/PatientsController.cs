using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Interfaces;

namespace PatientManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PatientFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await _patientService.GetAllAsync(filter, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:int}", Name = "GetPatientById")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var response = await _patientService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePatientDto dto, CancellationToken cancellationToken)
    {
        var response = await _patientService.CreateAsync(dto, cancellationToken);
        return CreatedAtRoute("GetPatientById", new { id = response.Data!.PatientId }, response);
    }
}
