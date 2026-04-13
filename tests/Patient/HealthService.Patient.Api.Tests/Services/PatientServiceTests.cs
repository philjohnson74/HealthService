using HealthService.Patient.Api.Data;
using HealthService.Patient.Api.Services;
using Moq;
using Xunit;

namespace HealthService.Patient.Api.Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _repositoryMock;
    private readonly PatientService _sut;

    public PatientServiceTests()
    {
        _repositoryMock = new Mock<IPatientRepository>();
        _sut = new PatientService(_repositoryMock.Object);
    }

    [Fact]
    public void GetById_WhenPatientExists_ReturnsPatient()
    {
        var expected = new Patient { Id = 1, Name = "Alice Hartley", NHSNumber = "485 777 3456" };
        _repositoryMock.Setup(r => r.GetById(1)).Returns(expected);

        var result = _sut.GetById(1);

        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
        Assert.Equal(expected.NHSNumber, result.NHSNumber);
    }

    [Fact]
    public void GetById_WhenPatientDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetById(99)).Returns((Patient?)null);

        var result = _sut.GetById(99);

        Assert.Null(result);
    }

    [Fact]
    public void GetById_DelegatesToRepository()
    {
        _sut.GetById(42);

        _repositoryMock.Verify(r => r.GetById(42), Times.Once);
    }
}
