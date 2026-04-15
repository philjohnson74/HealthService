using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HealthService.Patient.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HealthService.Patient.Api.Tests.Integration;

public class PatientEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PatientEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatientById_WhenPatientExists_Returns200()
    {
        var response = await _client.GetAsync("/patients/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientExists_ReturnsCorrectPatientData()
    {
        var response = await _client.GetAsync("/patients/1");
        var patient = await response.Content.ReadFromJsonAsync<PatientResponse>(JsonOptions);

        Assert.NotNull(patient);
        Assert.Equal(1, patient.Id);
        Assert.Equal("Alice Hartley", patient.Name);
        Assert.Equal("485 777 3456", patient.NhsNumber);
        Assert.Equal("Northside Medical Centre", patient.GpPractice);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientExists_ReturnsDateOnlyDateOfBirth()
    {
        var response = await _client.GetAsync("/patients/1");
        var json = await response.Content.ReadAsStringAsync();

        // Asserts DateOfBirth is serialised as a date-only string with no time component
        Assert.Contains("\"dateOfBirth\":\"1985-03-12\"", json);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientDoesNotExist_Returns404()
    {
        var response = await _client.GetAsync("/patients/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientDoesNotExist_ReturnsErrorResponseWithMessage()
    {
        var response = await _client.GetAsync("/patients/999");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions);

        Assert.NotNull(error);
        Assert.Contains("999", error.Message);
    }

    [Fact]
    public async Task GetPatientById_WhenIdIsNotAnInteger_Returns404()
    {
        // The :int route constraint means a non-integer ID does not match the route at all,
        // resulting in 404 (no route found) rather than 400 (bad request)
        var response = await _client.GetAsync("/patients/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
