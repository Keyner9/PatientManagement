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
}
