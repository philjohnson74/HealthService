using HealthService.Patient.Api.Data;
using Xunit;

namespace HealthService.Patient.Api.Tests.Data;

public class InMemoryPatientRepositoryTests
{
    private readonly InMemoryPatientRepository _sut = new();

    [Theory]
    [InlineData(1, "Alice Hartley",  "485 777 3456")]
    [InlineData(2, "Ben Okafor",     "321 654 9870")]
    [InlineData(3, "Clara Mendez",   "654 321 1234")]
    [InlineData(4, "David Nguyen",   "789 012 3456")]
    [InlineData(5, "Eleanor Walsh",  "147 258 3690")]
    public void GetById_WhenPatientExists_ReturnsCorrectPatient(int id, string expectedName, string expectedNhsNumber)
    {
        var result = _sut.GetById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedName, result.Name);
        Assert.Equal(expectedNhsNumber, result.NHSNumber);
    }

    [Fact]
    public void GetById_WhenPatientDoesNotExist_ReturnsNull()
    {
        var result = _sut.GetById(999);

        Assert.Null(result);
    }
}
