using HealthService.Patient.Api.Models;
using Xunit;

namespace HealthService.Patient.Api.Tests.Models;

public class PatientResponseTests
{
    private readonly Patient _patient = new()
    {
        Id = 1,
        NHSNumber = "485 777 3456",
        Name = "Alice Hartley",
        DateOfBirth = new DateTime(1985, 3, 12, 9, 30, 0),
        GPPractice = "Northside Medical Centre"
    };

    [Fact]
    public void From_MapsAllFieldsCorrectly()
    {
        var result = PatientResponse.From(_patient);

        Assert.Equal(_patient.Id, result.Id);
        Assert.Equal(_patient.NHSNumber, result.NhsNumber);
        Assert.Equal(_patient.Name, result.Name);
        Assert.Equal(_patient.GPPractice, result.GpPractice);
    }

    [Fact]
    public void From_ConvertsDateOfBirthToDateOnly()
    {
        var result = PatientResponse.From(_patient);

        Assert.Equal(DateOnly.FromDateTime(_patient.DateOfBirth), result.DateOfBirth);
        Assert.Equal(new DateOnly(1985, 3, 12), result.DateOfBirth);
    }

    [Fact]
    public void From_StripsTimeComponentFromDateOfBirth()
    {
        // Ensures a DateTime with a non-midnight time doesn't leak into the response
        var result = PatientResponse.From(_patient);

        Assert.Equal(1985, result.DateOfBirth.Year);
        Assert.Equal(3, result.DateOfBirth.Month);
        Assert.Equal(12, result.DateOfBirth.Day);
    }
}
