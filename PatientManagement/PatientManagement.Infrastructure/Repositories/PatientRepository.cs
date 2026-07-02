using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == id, cancellationToken);
    }

    public async Task<(IEnumerable<Patient> Items, int TotalCount)> GetAllAsync(
        string? name, string? documentNumber, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p =>
                p.FirstName.Contains(name) || p.LastName.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            query = query.Where(p => p.DocumentNumber == documentNumber);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .AnyAsync(p => p.DocumentType == documentType && p.DocumentNumber == documentNumber, cancellationToken);
    }

    public async Task<Patient> CreateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);
        return patient;
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
