namespace PatientManagement.Application.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
