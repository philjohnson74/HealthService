using HealthService.Patient.Api.Data;

namespace HealthService.Patient.Api.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository repository, ILogger<PatientService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Patient? GetById(int id)
    {
        var patient = _repository.GetById(id);

        if (patient is null)
        {
            _logger.LogWarning("Patient with ID {PatientId} was not found", id);
            return null;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("Patient with ID {PatientId} retrieved successfully", id);
        return patient;
    }
}
