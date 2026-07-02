namespace PatientManagement.Application.Exceptions;

public class DuplicatePatientException(string message) : Exception(message)
{
}
