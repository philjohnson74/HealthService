using HealthService.Patient.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IPatientRepository, InMemoryPatientRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/patients/{id:int}", (int id, IPatientRepository repository) =>
{
    var patient = repository.GetById(id);
    return patient is not null
        ? Results.Ok(patient)
        : Results.NotFound(new { message = $"Patient with ID {id} was not found." });
})
.WithName("GetPatientById");

app.Run();
