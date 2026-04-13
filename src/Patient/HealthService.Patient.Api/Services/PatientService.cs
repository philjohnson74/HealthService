using HealthService.Patient.Api.Data;

namespace HealthService.Patient.Api.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public Patient? GetById(int id) => _repository.GetById(id);
}
