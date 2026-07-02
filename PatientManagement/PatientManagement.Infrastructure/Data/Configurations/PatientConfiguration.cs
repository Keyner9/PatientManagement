using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.PatientId);

        builder.Property(p => p.PatientId)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.DocumentType)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.DocumentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(p => p.BirthDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(120);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(p => new { p.DocumentType, p.DocumentNumber })
            .IsUnique()
            .HasDatabaseName("UIX_Patients_DocumentType_DocumentNumber");
    }
}
