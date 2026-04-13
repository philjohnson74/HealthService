namespace HealthService.Patient.Api.Data;

public interface IPatientRepository
{
    Patient? GetById(int id);
}
