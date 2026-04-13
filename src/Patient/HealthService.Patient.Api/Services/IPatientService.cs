namespace HealthService.Patient.Api.Services;

public interface IPatientService
{
    Patient? GetById(int id);
}
