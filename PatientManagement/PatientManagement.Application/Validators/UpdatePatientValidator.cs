using FluentValidation;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Validators;

public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("El tipo de documento es obligatorio.")
            .MaximumLength(10).WithMessage("El tipo de documento no debe exceder 10 caracteres.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El numero de documento es obligatorio.")
            .MaximumLength(20).WithMessage("El numero de documento no debe exceder 20 caracteres.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(80).WithMessage("El nombre no debe exceder 80 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(80).WithMessage("El apellido no debe exceder 80 caracteres.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El numero de telefono no debe exceder 20 caracteres.")
            .When(x => x.PhoneNumber is not null);

        RuleFor(x => x.Email)
            .MaximumLength(120).WithMessage("El correo electronico no debe exceder 120 caracteres.")
            .EmailAddress().WithMessage("Formato de correo electronico invalido.")
            .When(x => x.Email is not null);
    }
}
