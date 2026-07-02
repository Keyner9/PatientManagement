using PatientManagement.Application.DTOs;
using PatientManagement.Application.Validators;

namespace PatientManagement.Tests;

public class UpdatePatientValidatorTests
{
    private readonly UpdatePatientValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotReturnError_WhenAllFieldsAreValid()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15),
            PhoneNumber = "555-0101",
            Email = "carlos@email.com"
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDocumentTypeIsEmpty()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = string.Empty,
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.DocumentType));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDocumentTypeExceedsMaxLength()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = new string('A', 11),
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.DocumentType));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDocumentNumberIsEmpty()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = string.Empty,
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.DocumentNumber));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDocumentNumberExceedsMaxLength()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = new string('A', 21),
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.DocumentNumber));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenFirstNameIsEmpty()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = string.Empty,
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.FirstName));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenFirstNameExceedsMaxLength()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = new string('A', 81),
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.FirstName));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenLastNameIsEmpty()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = string.Empty,
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.LastName));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenLastNameExceedsMaxLength()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = new string('A', 81),
            BirthDate = new DateOnly(1985, 3, 15)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.LastName));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenBirthDateIsDefault()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = default
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.BirthDate));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPhoneNumberExceedsMaxLength()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15),
            PhoneNumber = new string('A', 21)
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.PhoneNumber));
    }

    [Fact]
    public void Validate_ShouldNotReturnError_WhenPhoneNumberIsNull()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15),
            PhoneNumber = null
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenEmailIsInvalid()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15),
            Email = "correo-invalido"
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.Email));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenEmailExceedsMaxLength()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15),
            Email = new string('a', 121) + "@test.com"
        };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePatientDto.Email));
    }

    [Fact]
    public void Validate_ShouldNotReturnError_WhenEmailIsNull()
    {
        var dto = new UpdatePatientDto
        {
            DocumentType = "DNI",
            DocumentNumber = "12345678",
            FirstName = "Carlos",
            LastName = "Garcia",
            BirthDate = new DateOnly(1985, 3, 15),
            Email = null
        };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }
}
