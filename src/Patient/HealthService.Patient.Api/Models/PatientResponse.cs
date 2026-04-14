namespace HealthService.Patient.Api.Models;

public record PatientResponse(
    int Id,
    string NhsNumber,
    string Name,
    DateOnly DateOfBirth,
    string GpPractice
)
{
    public static PatientResponse From(Patient patient) => new(
        patient.Id,
        patient.NHSNumber,
        patient.Name,
        DateOnly.FromDateTime(patient.DateOfBirth),
        patient.GPPractice
    );
}
