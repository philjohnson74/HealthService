using HealthService.Patient.Api.Models;
using HealthService.Patient.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HealthService.Patient.Api.Endpoints;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/patients/{id:int}", Results<Ok<PatientResponse>, NotFound<ErrorResponse>> (int id, IPatientService service) =>
        {
            var patient = service.GetById(id);
            return patient is not null
                ? TypedResults.Ok(PatientResponse.From(patient))
                : TypedResults.NotFound(new ErrorResponse($"Patient with ID {id} was not found."));
        })
        .WithName("GetPatientById")
        .Produces<PatientResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }
}
