using System.Text;
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdatePatientDto dto, CancellationToken cancellationToken)
    {
        var response = await _patientService.UpdateAsync(id, dto, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _patientService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:int}/appointments")]
    public async Task<IActionResult> GetAppointmentsAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var response = await _patientService.GetAppointmentsAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReportAsync([FromQuery] DateTime? createdAfter, CancellationToken cancellationToken)
    {
        var date = createdAfter ?? DateTime.MinValue;
        var response = await _patientService.GetPatientsCreatedAfterAsync(date, cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine("Id,TipoDocumento,NumeroDocumento,Nombre,Apellido,Telefono,Email,FechaCreacion");

        foreach (var p in response.Data!)
        {
            sb.AppendLine($"{p.PatientId},{p.DocumentType},{p.DocumentNumber},{p.FirstName},{p.LastName},{p.PhoneNumber ?? ""},{p.Email ?? ""},{p.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"pacientes_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }
}
