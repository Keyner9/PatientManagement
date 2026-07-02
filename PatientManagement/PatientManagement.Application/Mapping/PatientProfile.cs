using AutoMapper;
using PatientManagement.Application.DTOs;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Mapping;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientDto>();
        CreateMap<Patient, PatientListDto>();
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>();
    }
}
