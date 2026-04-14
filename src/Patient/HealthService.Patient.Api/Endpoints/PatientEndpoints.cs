using HealthService.Patient.Api.Models;
using HealthService.Patient.Api.Services;

namespace HealthService.Patient.Api.Endpoints;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/patients/{id:int}", (int id, IPatientService service) =>
        {
            var patient = service.GetById(id);
            return patient is not null
                ? Results.Ok(PatientResponse.From(patient))
                : Results.NotFound(new { message = $"Patient with ID {id} was not found." });
        })
        .WithName("GetPatientById");

        return app;
    }
}
